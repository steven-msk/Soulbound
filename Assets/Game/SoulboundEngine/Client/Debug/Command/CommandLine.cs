using SoulboundEngine.Client.Debug.Commands;
using SoulboundEngine.Client.Debug.Commands.View;
using SoulboundEngine.Client.Input;
using SoulboundEngine.Client.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SoulboundEngine.Client.Debug {
	public sealed class CommandLine : ToggleableOverlay<UIHandledOverlayNode<ICommandLineHandler>>, IInputEventHandler {
		int IInputEventHandler.priority => 5005;
		private readonly CommandProcessor commandProcessor;
		private readonly List<string> history = new();

		public CommandLine(CommandProcessor commandProcessor) {
			this.commandProcessor = commandProcessor;
		}

		public void Show() {
			if (!this.visible) {
				this.visible = true;
				this.CreateNodeIfNull();
			}
		}

		public override void Toggle() {
			if (this.visible) this.node.Destroy();
			else this.Show();
		}

		private void SubmitCommand(string command) {
			this.node.Destroy();
			this.commandProcessor.SubmitCommand(command);
			this.history.Add(command);
		}

		IEnumerable<InputEventListener> IInputEventHandler.GetListeners() {
			InputEventListener GetKeyListener(InputToken token, Key key) {
				return InputEventListener.ConsumeAny(token, _ => this.HandleKey(key), priority: int.MaxValue);
			}

			return new[] {
				GetKeyListener(InputTokens.Keyboard.TAB, Key.Tab),
				GetKeyListener(InputTokens.Keyboard.ARROW_UP, Key.UpArrow),
				GetKeyListener(InputTokens.Keyboard.ARROW_DOWN, Key.DownArrow),
				GetKeyListener(InputTokens.Keyboard.ESC, Key.Escape),
				GetKeyListener(InputTokens.Keyboard.BACKSPACE, Key.Backspace),
			};
		}

		private void HandleKey(Key key) => this.node?.handle.HandleKey(key);

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
			handler.Init(inputField, this.commandProcessor, this.history);
			handler.shouldCloseCommandLine += this.Toggle;

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
			inputField.onSubmit.AddListener(this.SubmitCommand);
			inputField.onFocusSelectAll = false;
			inputField.restoreOriginalTextOnEscape = false;
			inputField.onDeselect.AddListener(_ => inputField.ActivateInputField());

			// now the caret object will be created
			obj.SetActive(true);

			return new UIHandledOverlayNode<ICommandLineHandler>(obj, handler);
		}

	}
}
