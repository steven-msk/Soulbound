using Brigadier.NET.Suggestion;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Logger = SoulboundEngine.Client.Debug.Logging.Logger;

namespace SoulboundEngine.Client.Debug.Commands.View {
	public sealed class CommandLineHandler : MonoBehaviour, ICommandLineHandler {
		private CommandLineInputField inputField;
		private CommandProcessor commandProcessor;
		private readonly CompletionManager completionQueue = new();
		private List<string> history;
		private VerticalLayoutGroup completionPanel;
		private int historyIndex;
		private CommandInputMode currentInputMode;
		public event Action shouldCloseCommandLine;
		private TextMeshProUGUI[] currentCompletions;
		private int lastCaretPos = 0;
		private bool eligibleForHistoryCycling;
		private bool hasEnteredText = false;

		public void Init(CommandLineInputField inputField, CommandProcessor commandProcessor, IEnumerable<string> history) {
			this.inputField = inputField;
			this.commandProcessor = commandProcessor;
			this.history = history.ToList();
			inputField.ShouldBlockNavigation = () => true;

			// activate and prefix text next frame
			// so TMP can finish internal processes
			UniTask.Post(async () => {
				await UniTask.NextFrame();

				inputField.ActivateInputField();
				inputField.SetTextWithoutNotify("/");
				inputField.ForceLabelUpdate();
				inputField.onValueChanged.AddListener(value => {
					if (!this.hasEnteredText) this.hasEnteredText = value.Length > 1;
					if (value.Equals("")) this.hasEnteredText = false;
					this.eligibleForHistoryCycling = !this.hasEnteredText || this.currentInputMode == CommandInputMode.CyclingHistory;
					if (!this.eligibleForHistoryCycling) this.currentInputMode = CommandInputMode.Typing;
				});
				this.SetCaretToEnd();
				if (history.Any()) this.currentInputMode = CommandInputMode.CyclingHistory;
				this.hasEnteredText = false;
			});
		}

		private void Update() {
			if (this.lastCaretPos != this.inputField.caretPosition) {
				this.ShowCompletions(this.inputField.text);
			}
			this.lastCaretPos = this.inputField.caretPosition;
		}

		private void SetCaretToEnd() {
			int end = this.inputField.text.Length;
			this.inputField.caretPosition = end;
			this.inputField.selectionAnchorPosition = end;
			this.inputField.selectionFocusPosition = end;
		}

		// TODO: refactor command line handler logic
		void ICommandLineHandler.HandleKey(Key key) {
			if (key == Key.Escape) {
				shouldCloseCommandLine();
				return;
			}
			if (key != Key.Tab && key != Key.UpArrow && key != Key.DownArrow) this.currentInputMode = CommandInputMode.Typing;
			switch (this.currentInputMode) {
				case CommandInputMode.Typing:
					this.HandleTyping(key);
					break;
				case CommandInputMode.CyclingCompletions:
					this.HandleCompletion(key);
					break;
				case CommandInputMode.CyclingHistory:
					this.HandleHistory(key);
					break;
			}
		}

		private void HandleTyping(Key key) {
			if ((key == Key.Tab || key == Key.UpArrow || key == Key.DownArrow)
					&& this.completionQueue.GetCompletionCount() > 0) {
				this.currentInputMode = CommandInputMode.CyclingCompletions;
				this.HandleCompletion(key);
			} else if ((key == Key.UpArrow || key == Key.DownArrow) && this.history.Any() && this.eligibleForHistoryCycling) {
				this.currentInputMode = CommandInputMode.CyclingHistory;
				this.HandleHistory(key);
			}
		}

		private void HandleCompletion(Key key) {
			if (key == Key.DownArrow) {
				this.HighlightCompletion(this.completionQueue.SelectNext());
			} else if (key == Key.UpArrow) {
				this.HighlightCompletion(this.completionQueue.SelectPrevious());
			} else if (key == Key.Tab) {
				this.InsertCompletion();
			} 
		}

		private void HandleHistory(Key key) {
			if (key == Key.UpArrow) {
				this.historyIndex--;
				if (this.historyIndex < 0) this.historyIndex = this.history.Count - 1;
				this.InsertHistory();
			} else if (key == Key.DownArrow) {
				this.historyIndex = (this.historyIndex + 1) % this.history.Count;
				this.InsertHistory();
			}
		}

		public void InsertCompletion()	{
			if (this.completionQueue.GetCompletionCount() == 0) return;

			Suggestion suggestion = this.completionQueue.GetSelected();

			string withoutLeadingSlash = this.inputField.text[1..];
			this.inputField.text = $"/{suggestion.Apply(withoutLeadingSlash)}";

			int newCaret = suggestion.Range.Start + 1 + suggestion.Text.Length;
			this.inputField.caretPosition = newCaret;
			this.inputField.selectionAnchorPosition = newCaret;
			this.inputField.selectionFocusPosition = newCaret;
		}

		private void InsertHistory() {
			this.inputField.text = this.history[this.historyIndex];
			this.SetCaretToEnd();
		}

		private void ExitHistory() {
			this.currentInputMode = CommandInputMode.Typing;
		}

		public void ShowCompletions(string value) {
			if (this.completionPanel == null) this.completionPanel = this.CreateCompletionPanel();

			int caretPos = this.inputField.caretPosition;

			this.commandProcessor.GetCompletions(value, caretPos)
				.ContinueWith(suggestions => {
					if (suggestions.List.Any()) {
						this.completionPanel.gameObject.SetActive(true);
						foreach (var component in this.completionPanel.GetComponentsInChildren<TextMeshProUGUI>()) {
							component.gameObject.SetActive(false);
						}
					} else this.completionPanel.gameObject.SetActive(false);

					this.currentCompletions = this.completionPanel.GetComponentsInChildren<TextMeshProUGUI>(true);
					this.completionQueue.SetCompletions(suggestions.List);

					int i = 0;
					for (; i < this.currentCompletions.Length && i < suggestions.List.Count; i++) {
						this.currentCompletions[i].gameObject.SetActive(true);
						this.currentCompletions[i].text = suggestions.List[i].Text;
						this.currentCompletions[i].Rebuild(CanvasUpdate.LatePreRender);
					}

					for (; i < suggestions.List.Count; i++) {
						this.CreateCompletionComponent(suggestions.List[i].Text);
					}
					this.currentCompletions = this.completionPanel.GetComponentsInChildren<TextMeshProUGUI>(true);
					this.HighlightCompletion(this.completionQueue.GetSelectedIndex());
				}).Forget(e => {
					this.completionQueue.SetCompletions(Array.Empty<Suggestion>().ToList());
					this.completionPanel.gameObject.SetActive(false);
					Logger.LogFatal(e);
				});

		}

		private void HighlightCompletion(int index) {
			if (this.InvalidCompletionPanel()) return;
			for (int i = 0; i < this.currentCompletions.Length; i++) {
				this.RevokeSelectedLayout(this.currentCompletions[i]);
			}
			if (index != -1) this.ApplySelectedLayout(this.currentCompletions[index]);
			this.RebuildPanelLayout();
		}

		private bool InvalidCompletionPanel() {
			return this.completionPanel == null
				|| (this.completionPanel != null && !this.completionPanel.gameObject.activeSelf);
		}

		private void RebuildPanelLayout() {
			if (!this.InvalidCompletionPanel()) {
				LayoutRebuilder.ForceRebuildLayoutImmediate(
					this.completionPanel.GetComponent<RectTransform>()
				);
			}
		}

		private VerticalLayoutGroup CreateCompletionPanel() {
			GameObject obj = new("Completion Panel", typeof(RectTransform));
			obj.transform.SetParent(this.transform, false);

			RectTransform rect = obj.GetComponent<RectTransform>();
			rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
			rect.pivot = Vector2.zero;
			rect.anchoredPosition = Vector2.zero;

			VerticalLayoutGroup layout = obj.AddComponent<VerticalLayoutGroup>();
			layout.childControlWidth = layout.childControlHeight = false;
			layout.childForceExpandWidth = layout.childForceExpandHeight = false;
			layout.childScaleWidth = layout.childScaleHeight = true;

			ContentSizeFitter fitter = obj.AddComponent<ContentSizeFitter>();
			fitter.verticalFit = fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

			obj.SetActive(true);
			return layout;
		}

		private void CreateCompletionComponent(string completion) {
			GameObject obj = new("Completion", typeof(RectTransform));
			obj.transform.SetParent(this.completionPanel.transform, false);

			TextMeshProUGUI text = obj.AddComponent<TextMeshProUGUI>();
			text.fontSize = 15f;
			text.text = completion;

			ContentSizeFitter fitter = obj.AddComponent<ContentSizeFitter>();
			fitter.verticalFit = fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
		}

		private void ApplySelectedLayout(TextMeshProUGUI text) {
			text.fontStyle = FontStyles.Bold;
		}

		private void RevokeSelectedLayout(TextMeshProUGUI text) {
			text.fontStyle = FontStyles.Normal;
		}
	}
}
