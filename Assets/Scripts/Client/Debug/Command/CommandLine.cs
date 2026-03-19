using SoulboundBackend.Client.Debug.Commands;
using SoulboundBackend.Client.Debug.Commands.View;
using SoulboundBackend.Client.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SoulboundBackend.Client.Debug {
	public sealed class CommandLine : ToggleableOverlay<UIHandledOverlayNode<ICommandLineHandler>> {
		private readonly CommandProcessor commandProcessor;
		private readonly List<string> history = new();

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
			history.Add(command);
		}

		public void HandleKey(Key key) => node?.handle.HandleKey(key);

		protected override UIHandledOverlayNode<ICommandLineHandler> GetNode() {
			GameObject obj = new("Command Line", typeof(RectTransform));

			// TMP_InputField misses the condition to create the caret object when OnEnable is called
			// the object needs to be disabled before the component is added
			obj.SetActive(false);

			RectTransform rect = obj.GetComponent<RectTransform>();
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = new Vector2(1f, 0f);
			rect.pivot = new Vector2(0.5f, 0f);
			rect.sizeDelta = new Vector2(0f, 30f);

			Image bg = obj.AddComponent<Image>();
			bg.color = new Color(0.1f, 0.1f, 0.1f, 1f);

			CommandLineInputField inputField = obj.AddComponent<CommandLineInputField>();

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
			handler.Init(inputField, commandProcessor, history);
			handler.shouldCloseCommandLine += Toggle;

			GameObject viewport = new("Viewport", typeof(RectTransform), typeof(Mask), typeof(Image));
			viewport.transform.SetParent(obj.transform, false);

			Mask mask = viewport.GetComponent<Mask>();
			mask.showMaskGraphic = false;

			RectTransform viewportRect = viewport.GetComponent<RectTransform>();
			viewportRect.anchorMin = Vector2.zero;
			viewportRect.anchorMax = Vector2.one;
			viewportRect.offsetMin = Vector2.zero;
			viewportRect.offsetMax = Vector2.zero;

			textObj.transform.SetParent(viewportRect.transform, false);

			inputField.textComponent = text;
			inputField.textViewport = viewportRect;
			inputField.lineType = TMP_InputField.LineType.SingleLine;
			inputField.onSubmit.AddListener(SubmitCommand);
			inputField.onFocusSelectAll = false;
			inputField.restoreOriginalTextOnEscape = false;
			inputField.onDeselect.AddListener(_ => inputField.ActivateInputField());

			// now the caret object will be created
			obj.SetActive(true);

			return new UIHandledOverlayNode<ICommandLineHandler>(obj, handler);
		}

	}
}
