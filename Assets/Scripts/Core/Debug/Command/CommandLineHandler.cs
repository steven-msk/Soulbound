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
using UnityEngine.UI;
using Logger = SoulboundBackend.Core.Debug.Logging.Logger;

namespace SoulboundBackend.Core.Debug.Commands {
	public sealed class CommandLineHandler : MonoBehaviour, ICommandLineHandler {
		private TMP_InputField inputField;
		private CommandProcessor commandProcessor;
		private readonly CommandCompletion completion = new();
		private GameObject completionPanel;

		public void Init(TMP_InputField inputField, CommandProcessor commandProcessor) {
			this.inputField = inputField;
			this.commandProcessor = commandProcessor;
			StartCoroutine(ActivateNextFrame());
		}

		IEnumerator ActivateNextFrame() {
			yield return null;

			inputField.ActivateInputField();
			inputField.SetTextWithoutNotify("/");
			inputField.ForceLabelUpdate();

			yield return null;
			SetCaretToEnd();
		}

		private void SetCaretToEnd() {
			int end = inputField.text.Length;
			inputField.caretPosition = end;
			inputField.selectionAnchorPosition = end;
			inputField.selectionFocusPosition = end;
		}

		void ICommandLineHandler.InsertCompletion() {
			if(InvalidCompletionState()) return;

			CommandCompletionToken completionToken = completion.GetSelected().Value;
			string append = inputField.text + completionToken.value[completionToken.start..] + " ";
			inputField.text = append;
			SetCaretToEnd();
		}

		public void ShowCompletions(string value) {
			if (completionPanel == null) completionPanel = CreateCompletionPanel();

			List<CommandCompletionToken> completions = commandProcessor.GetCompletions(value).ToList();
			int previousSelected = completion.GetSelectedIndex();
			completion.SetCompletions(completions);
			if (completions.Any()) {
				completionPanel.SetActive(true);
				foreach (var component in completionPanel.GetComponentsInChildren<TextMeshProUGUI>()) {
					component.gameObject.SetActive(false);
				}
			}
			else {
				completionPanel.SetActive(false);
				return;
			}

			TextMeshProUGUI[] pool = completionPanel.GetComponentsInChildren<TextMeshProUGUI>(true);

			int i = 0;
			for (; i < pool.Length && i < completions.Count; i++) {
				pool[i].gameObject.SetActive(true);
				pool[i].text = completions[i].value;
			}

			for (; i < completions.Count; i++) {
				CreateCompletionComponent(completions[i].value);
			}

			int currentSelected = completion.GetSelectedIndex();
			TextMeshProUGUI[] components = completionPanel.GetComponentsInChildren<TextMeshProUGUI>();
			if (previousSelected != -1 && previousSelected < components.Length) {
				UpdateSelectedCompletion(previousSelected, currentSelected);
			} else {
				ApplySelectedLayout(components[currentSelected]);
			}
		}

		void ICommandLineHandler.SelectNextCompletion() {
			if (InvalidCompletionState()) return;

			int previousIndex = completion.GetSelectedIndex();
			int index = completion.SelectNext();
			UpdateSelectedCompletion(previousIndex, index);
		}

		void ICommandLineHandler.SelectPreviousCompletion() {
			if (InvalidCompletionState()) return;

			int previousIndex = completion.GetSelectedIndex();
			int index = completion.SelectPrevious();
			UpdateSelectedCompletion(previousIndex, index);
		}

		private void UpdateSelectedCompletion(int previousIndex, int currentIndex) {
			TextMeshProUGUI[] components = completionPanel.GetComponentsInChildren<TextMeshProUGUI>();
			RevokeSelectedLayout(components[previousIndex]);
			ApplySelectedLayout(components[currentIndex]);
		}

		private bool InvalidCompletionState() {
			return (completionPanel != null && !completionPanel.activeSelf)
				|| completion.GetSelectedIndex() == -1;
		}

		private GameObject CreateCompletionPanel() {
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
			return obj;
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
