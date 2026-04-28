using SoulboundEngine.Client.UI.Containers;
using System;
using TMPro;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.UI {
	public sealed class InputFieldBuilder : IUIElementHandleBuilder<InputFieldHandle> {
		private readonly IUIElementTemplate<InputFieldHandle> template;
		private bool built = false;
		private string? placeholder = null;

		public InputFieldBuilder(IUIElementTemplate<InputFieldHandle> template) {
			this.template = template;
		}

		public InputFieldBuilder Placeholder(string text) {
			this.placeholder = text;
			return this;
		}

		public InputFieldHandle Build(IUIElementContainer container) {
			if (this.built) throw new InvalidOperationException("Button already built");
			this.built = true;

			GameObject obj = this.template.Instantiate();
			InputFieldHandle handle = obj.AddComponent<InputFieldHandle>();
			TMP_InputField field = handle.GetComponent<TMP_InputField>();
			handle.OnBuild(field);
			if (this.placeholder != null) {
				GameObject placeholderObj = new("Placeholder", typeof(RectTransform));
				placeholderObj.transform.SetParent(obj.transform, false);

				TextMeshProUGUI placeholder = placeholderObj.AddComponent<TextMeshProUGUI>();
				placeholder.fontSize = 16f;
				placeholder.color = Color.gray;
				placeholder.text = this.placeholder;
				placeholder.fontStyle = FontStyles.Italic;

				RectTransform rect = placeholderObj.GetComponent<RectTransform>();
				rect.anchorMin = rect.anchorMax = Vector2.zero;
				rect.pivot = new Vector2(0f, 0.5f);
				rect.anchoredPosition = new Vector2(8f, 0f);

				field.placeholder = placeholder;
			}

			UIElementNode node = new(obj);
			container.AddElement(node);

			this.template.Apply(handle);

			return handle;
		}
	}
}
