using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace SoulboundBackend.Core.Debug.Commands {
	public sealed class CommandLineHandler : MonoBehaviour, ICommandLineHandler {
		private TMP_InputField inputField;

		public void Init(TMP_InputField inputField) {
			this.inputField = inputField;
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
		}
	}
}
