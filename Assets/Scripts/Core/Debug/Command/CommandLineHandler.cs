using SoulboundBackend.Core.Debug.Logging;
using System;
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
			inputField.text = "/";
			inputField.onValueChanged.AddListener(ForceLeadingSlash);
		}

		private void ForceLeadingSlash(string value) {
			inputField.onValueChanged.RemoveListener(ForceLeadingSlash);
			inputField.text = $"/{value}";
			SetCaretToEnd();
		}

		private void SetCaretToEnd() {
			int end = inputField.text.Length;
			inputField.caretPosition = end;
			inputField.selectionAnchorPosition = end;
			inputField.selectionFocusPosition = end;
		}

		public void ValueChanged(string value) {
			foreach (var completion in commandProcessor.GetCompletions(value)) {
				Logger.LogInfo(completion);
			}
			Logger.LogInfo("");

			if (completionPanel == null) completionPanel = CreateCompletionPanel();

			List<string> completions = commandProcessor.GetCompletions(value).ToList();
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
				pool[i].text = completions[i];
			}

			for (; i < completions.Count; i++) {
				CreateCompletionComponent(completions[i]);
			}
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
	}
}
