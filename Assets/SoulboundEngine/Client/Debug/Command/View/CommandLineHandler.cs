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
		private readonly CommandCompletion completionQueue = new();
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
					if (!hasEnteredText) hasEnteredText = value.Length > 1;
					if (value.Equals("")) hasEnteredText = false;
					eligibleForHistoryCycling = !hasEnteredText || currentInputMode == CommandInputMode.CyclingHistory;
					if (!eligibleForHistoryCycling) currentInputMode = CommandInputMode.Typing;
				});
				SetCaretToEnd();
				if (history.Any()) currentInputMode = CommandInputMode.CyclingHistory;
				hasEnteredText = false;
			});
		}

		private void Update() {
			if (lastCaretPos != inputField.caretPosition) {
				ShowCompletions(inputField.text);
			}
			lastCaretPos = inputField.caretPosition;
		}

		private void SetCaretToEnd() {
			int end = inputField.text.Length;
			inputField.caretPosition = end;
			inputField.selectionAnchorPosition = end;
			inputField.selectionFocusPosition = end;
		}

		// TODO: refactor command line handler logic
		void ICommandLineHandler.HandleKey(Key key) {
			if (key == Key.Escape) {
				shouldCloseCommandLine();
				return;
			}
			if (key != Key.Tab && key != Key.UpArrow && key != Key.DownArrow) currentInputMode = CommandInputMode.Typing;
			switch (currentInputMode) {
				case CommandInputMode.Typing: HandleTyping(key);
					break;
				case CommandInputMode.CyclingCompletions: HandleCompletion(key);
					break;
				case CommandInputMode.CyclingHistory: HandleHistory(key);
					break;
			}
		}

		private void HandleTyping(Key key) {
			if ((key == Key.Tab || key == Key.UpArrow || key == Key.DownArrow)
					&& completionQueue.GetCompletionCount() > 0) {
				currentInputMode = CommandInputMode.CyclingCompletions;
				HandleCompletion(key);
			} else if ((key == Key.UpArrow || key == Key.DownArrow) && history.Any() && eligibleForHistoryCycling) {
				currentInputMode = CommandInputMode.CyclingHistory;
				HandleHistory(key);
			}
		}

		private void HandleCompletion(Key key) {
			if (key == Key.DownArrow) {
				HighlightCompletion(completionQueue.SelectNext());
			} else if (key == Key.UpArrow) {
				HighlightCompletion(completionQueue.SelectPrevious());
			} else if (key == Key.Tab) {
				InsertCompletion();
			} 
		}

		private void HandleHistory(Key key) {
			if (key == Key.UpArrow) {
				historyIndex--;
				if (historyIndex < 0) historyIndex = history.Count - 1;
				InsertHistory();
			} else if (key == Key.DownArrow) {
				historyIndex = (historyIndex + 1) % history.Count;
				InsertHistory();
			}
		}

		public void InsertCompletion()	{
			if (completionQueue.GetCompletionCount() == 0) return;

			Suggestion suggestion = completionQueue.GetSelected();
			string insert = suggestion.Text;
	
			// TODO: fix inconsistent suggestion insertion

			string prefix = inputField.text[..suggestion.Range.Start];
			//int suffixStart = Mathf.Min(inputField.text.Length, suggestion.Range.Start + suggestion.Range.Length);
			//string suffix = inputField.text[suffixStart..];

			//string result = prefix + insert + suffix;
			inputField.text = suggestion.Apply(inputField.text);

			int newCaret = prefix.Length + insert.Length;
			inputField.caretPosition = newCaret;
			inputField.selectionAnchorPosition = newCaret;
			inputField.selectionFocusPosition = newCaret;
		}

		private void InsertHistory() {
			inputField.text = history[historyIndex];
			SetCaretToEnd();
		}

		private void ExitHistory() {
			currentInputMode = CommandInputMode.Typing;
		}

		public void ShowCompletions(string value) {
			if (completionPanel == null) completionPanel = CreateCompletionPanel();

			int caretPos = inputField.caretPosition;

			commandProcessor.GetCompletions(value, caretPos)
				.ContinueWith(suggestions => {
					if (suggestions.List.Any()) {
						completionPanel.gameObject.SetActive(true);
						foreach (var component in completionPanel.GetComponentsInChildren<TextMeshProUGUI>()) {
							component.gameObject.SetActive(false);
						}
					} else completionPanel.gameObject.SetActive(false);

					currentCompletions = completionPanel.GetComponentsInChildren<TextMeshProUGUI>(true);
					completionQueue.SetCompletions(suggestions.List);

					int i = 0;
					for (; i < currentCompletions.Length && i < suggestions.List.Count; i++) {
						currentCompletions[i].gameObject.SetActive(true);
						currentCompletions[i].text = suggestions.List[i].Text;
						currentCompletions[i].Rebuild(CanvasUpdate.LatePreRender);
					}

					for (; i < suggestions.List.Count; i++) {
						CreateCompletionComponent(suggestions.List[i].Text);
					}
					currentCompletions = completionPanel.GetComponentsInChildren<TextMeshProUGUI>(true);
					HighlightCompletion(completionQueue.GetSelectedIndex());
				}).Forget(e => {
					completionQueue.SetCompletions(Array.Empty<Suggestion>().ToList());
					completionPanel.gameObject.SetActive(false);
					Logger.LogFatal(e);
				});

		}

		private void HighlightCompletion(int index) {
			if (InvalidCompletionPanel()) return;
			for (int i = 0; i < currentCompletions.Length; i++) {
				RevokeSelectedLayout(currentCompletions[i]);
			}
			if (index != -1) ApplySelectedLayout(currentCompletions[index]);
			RebuildPanelLayout();
		}

		private bool InvalidCompletionPanel() {
			return completionPanel == null
				|| (completionPanel != null && !completionPanel.gameObject.activeSelf);
		}

		private void RebuildPanelLayout() {
			if (!InvalidCompletionPanel()) {
				LayoutRebuilder.ForceRebuildLayoutImmediate(
					completionPanel.GetComponent<RectTransform>()
				);
			}
		}

		private VerticalLayoutGroup CreateCompletionPanel() {
			GameObject obj = new("Completion Panel", typeof(RectTransform));
			obj.transform.SetParent(transform, false);

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
			obj.transform.SetParent(completionPanel.transform, false);

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
