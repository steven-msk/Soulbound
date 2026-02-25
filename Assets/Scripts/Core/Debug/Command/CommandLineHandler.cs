using SoulboundBackend.Core.Debug.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Logger = SoulboundBackend.Core.Debug.Logging.Logger;

namespace SoulboundBackend.Core.Debug.Commands {
	public sealed class CommandLineHandler : MonoBehaviour, ICommandLineHandler {
		private TMP_InputField inputField;
		private CommandProcessor commandProcessor;

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
		}
	}
}
