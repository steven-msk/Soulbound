using SoulboundEngine.Client.UI.Containers;
using System;
using TMPro;
using UnityEngine;

namespace SoulboundEngine.Client.UI {
	public sealed class InputFieldBuilder : IUIElementHandleBuilder<InputFieldHandle> {
		private readonly IUIElementTemplate<InputFieldHandle> template;
		private bool built = false;

		public InputFieldBuilder(IUIElementTemplate<InputFieldHandle> template) {
			this.template = template;
		}

		public InputFieldHandle Build(IUIElementContainer container) {
			if (this.built) throw new InvalidOperationException("Button already built");
			this.built = true;

			GameObject obj = this.template.Instantiate();
			InputFieldHandle handle = obj.AddComponent<InputFieldHandle>();
			handle.Build(handle.GetComponent<TMP_InputField>());

			UIElementNode node = new(obj);
			container.AddElement(node);

			this.template.Apply(handle);

			return handle;
		}
	}
}
