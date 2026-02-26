using SoulboundBackend.Client.Input;
using SoulboundBackend.Core.Debug.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Logger = SoulboundBackend.Core.Debug.Logging.Logger;

namespace SoulboundBackend.Core.Debug.Commands {
	public sealed class CommandLineHandler : MonoBehaviour, ICommandLineHandler {
		private CommandLineInputField inputField;
		private CommandProcessor commandProcessor;
		private readonly CommandCompletion completion = new();
		private List<string> history;
		private VerticalLayoutGroup completionPanel;
		private int historyIndex;
		private CommandInputMode currentInputMode;
		public event Action shouldCloseCommandLine;
		private TextMeshProUGUI[] currentCompletions;

		public void Init(CommandLineInputField inputField, CommandProcessor commandProcessor, IEnumerable<string> history) {
			this.inputField = inputField;
			this.commandProcessor = commandProcessor;
			this.history = history.ToList();
			inputField.ShouldBlockNavigation = () => true;
			StartCoroutine(ActivateNextFrame());
		}

		IEnumerator ActivateNextFrame() {
			yield return null;

			inputField.ActivateInputField();
			inputField.SetTextWithoutNotify("/");
			inputField.ForceLabelUpdate();
			ValueChanged("/");

			yield return null;
			SetCaretToEnd();
		}

		private void SetCaretToEnd() {
			int end = inputField.text.Length;
			inputField.caretPosition = end;
			inputField.selectionAnchorPosition = end;
			inputField.selectionFocusPosition = end;
		}

		void ICommandLineHandler.HandleKey(Key key) {
			if (key == Key.Escape
					&& completion.GetCompletionCount() == 0
					&& currentInputMode == CommandInputMode.Typing) {
				shouldCloseCommandLine();
				return;
			}
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
			if ((key == Key.Tab || key == Key.UpArrow || key == Key.DownArrow || key == Key.Escape)
					&& completion.GetCompletionCount() > 0) {
				currentInputMode = CommandInputMode.CyclingCompletions;
				HandleCompletion(key);
			} else if ((key == Key.UpArrow || key == Key.DownArrow) && history.Count > 0) {
				currentInputMode = CommandInputMode.CyclingHistory;
				HandleHistory(key);
			}
		}

		private void HandleCompletion(Key key) {
			if (key == Key.DownArrow) {
				HighlightCompletion(completion.SelectNext());
			} else if (key == Key.UpArrow) {
				HighlightCompletion(completion.SelectPrevious());
			} else if (key == Key.Tab) {
				InsertCompletion();
			} else if (key == Key.Escape) {
				ExitCompletion();
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
			} else if (key == Key.Escape) {
				ExitHistory();
			}  else {
				HandleTyping(key);
			}
		}

		public void InsertCompletion() {
			if (completion.GetCompletionCount() == 0) return;
			CommandCompletionToken completionToken = completion.GetSelected().Value;
			string append = inputField.text + completionToken.value[completionToken.start..];
			inputField.text = append;
			SetCaretToEnd();
		}

		private void InsertHistory() {
			inputField.text = history[historyIndex];
			SetCaretToEnd();
		}

		private void ExitCompletion() {
			Destroy(completionPanel.gameObject);
			completion.ClearCompletions();
			currentInputMode = CommandInputMode.Typing;
		}

		private void ExitHistory() {
			currentInputMode = CommandInputMode.Typing;
		}

		public void ValueChanged(string value) {
			ShowCompletions(value);
			HighlightCompletion(completion.GetSelectedIndex());
		}

		public void ShowCompletions(string value) {
			if (completionPanel == null) completionPanel = CreateCompletionPanel();

			List<CommandCompletionToken> completions = commandProcessor.GetCompletions(value).ToList();
			completion.SetCompletions(completions);

			if (completions.Any()) {
				completionPanel.gameObject.SetActive(true);
				foreach (var component in completionPanel.GetComponentsInChildren<TextMeshProUGUI>()) {
					component.gameObject.SetActive(false);
				}
			} else completionPanel.gameObject.SetActive(false);

			currentCompletions = completionPanel.GetComponentsInChildren<TextMeshProUGUI>(true);

			int i = 0;
			for (; i < currentCompletions.Length && i < completions.Count; i++) {
				currentCompletions[i].gameObject.SetActive(true);
				currentCompletions[i].text = completions[i].value;
				currentCompletions[i].Rebuild(CanvasUpdate.LatePreRender);
			}

			for (; i < completions.Count; i++) {
				CreateCompletionComponent(completions[i].value);
			}
			currentCompletions = completionPanel.GetComponentsInChildren<TextMeshProUGUI>(true);
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
