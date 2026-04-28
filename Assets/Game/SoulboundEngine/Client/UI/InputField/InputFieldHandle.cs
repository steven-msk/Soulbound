using TMPro;
using UnityEngine;

namespace SoulboundEngine.Client.UI {
	[RequireComponent(typeof(TMP_InputField))]
	public sealed class InputFieldHandle : MonoBehaviour, IUIElementHandle {
		private TMP_InputField field;

		public void OnBuild(TMP_InputField field) {
			this.field = field;
		}

		public void SetVisible(bool visible) {
			this.gameObject.SetActive(visible);
		}

		public string GetText() {
			return this.field.text;
		}

		public void Clear() {
			this.field.text = "";
		}
	}
}
