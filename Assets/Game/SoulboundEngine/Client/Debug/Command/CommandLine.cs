using SoulboundEngine.Client.Debug.Commands;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.Debug {
	public sealed class CommandLine {
		private VisualElement root;
		private TextField textField;
		private readonly CommandProcessor commandProcessor;
		private readonly List<string> history = new();
		private readonly SoulboundClient.DebugOverlayManager debugOverlayManager;

		public CommandLine(CommandProcessor commandProcessor, SoulboundClient.DebugOverlayManager debugOverlayManager) {
			this.debugOverlayManager = debugOverlayManager;
			this.commandProcessor = commandProcessor;
		}

		public bool isVisible { get; private set; }

		public void OnBind(VisualElement root) {
			this.root = root;
			this.textField = root.Q<TextField>("TextField");
		}

		public void Show() {
			if (this.isVisible) return;
			this.isVisible = true;
			this.root.style.display = DisplayStyle.Flex;
			this.textField.value = "/";
			this.textField.Focus();
		}

		public void Hide() {
			if (!this.isVisible) return;
			this.isVisible = false;
			this.root.style.display = DisplayStyle.None;
			this.textField.value = "/";
			this.debugOverlayManager.Hide(SoulboundClient.DebugOverlayFeature.CommandLine);
		}

		public bool HandleKey(Key key) {
			if (!this.isVisible) return false;

			if (key == Key.Escape) {
				this.Hide();
				return true;
			}

			if (key == Key.Enter) {
				string command = this.textField.value;
				this.SubmitCommand(command);
				this.Hide();
				return true;
			}

			//if (key != Key.Tab && key != Key.UpArrow && key != Key.DownArrow) currentInputMode = CommandInputMode.Typing;
			//switch (currentInputMode) {
			//	case CommandInputMode.Typing:
			//		HandleTyping(key);
			//		break;
			//	case CommandInputMode.CyclingCompletions:
			//		HandleCompletion(key);
			//		break;
			//	case CommandInputMode.CyclingHistory:
			//		HandleHistory(key);
			//		break;
			//}
			return false;
		}

		private void SubmitCommand(string command) {
			this.commandProcessor.SubmitCommand(command);
			this.history.Add(command);
		}


	}
}
