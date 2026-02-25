using SoulboundBackend.Client.UI;
using SoulboundBackend.Core.Debug.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoulboundBackend.Core.Debug {
	public sealed class CommandLine : ToggleableOverlay<UIOverlayNode> {
		private readonly CommandProcessor commandProcessor;

		public CommandLine(CommandProcessor commandProcessor) {
			this.commandProcessor = commandProcessor;
		}

		public void Show() {
			if (!visible) {
				visible = true;
				CreateNodeIfNull();
			}
		}

		public override void Toggle() {
			if (visible) node.Destroy();
			else Show();
		}

		private void SubmitCommand(string command) {
			node.Destroy();
			commandProcessor.SubmitCommand(command);
		}

		protected override UIOverlayNode GetNode() {
			GameObject obj = new("Command Line", typeof(RectTransform));

			RectTransform rect = obj.GetComponent<RectTransform>();
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = new Vector2(1f, 0f);
			rect.pivot = new Vector2(0.5f, 0f);
			rect.sizeDelta = new Vector2(0f, 30f);

			Image bg = obj.AddComponent<Image>();
			bg.color = new Color(0.1f, 0.1f, 0.1f, 1f);

			TMP_InputField inputField = obj.AddComponent<TMP_InputField>();

			GameObject textObj = new("Text", typeof(RectTransform));
			textObj.transform.SetParent(obj.transform, false);

			TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
			text.fontSize = 15f;
			text.color = Color.white;

			RectTransform textRect = textObj.GetComponent<RectTransform>();
			textRect.anchorMin = Vector2.zero;
			textRect.anchorMax = Vector2.one;
			textRect.offsetMin = new Vector2(10f, 0f);
			textRect.offsetMax = new Vector2(-10f, 0f);

			CommandLineHandler handler = obj.AddComponent<CommandLineHandler>();
			handler.Init(inputField);

			inputField.textComponent = text;
			inputField.lineType = TMP_InputField.LineType.SingleLine;
			inputField.onSubmit.AddListener(SubmitCommand);
			inputField.onValueChanged.AddListener(handler.ValueChanged);
			inputField.ActivateInputField();

			return new UIOverlayNode(obj);
		}

	}
}
