using SoulboundBackend.Client.UI;
using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI {
	public sealed class ButtonBuilder {
		private readonly GameObject prefab;
		private string text;
		private bool enabled = true;
		private Action onClick;
		private bool built;

		private ButtonBuilder(GameObject prefab) {
			this.prefab = prefab;
		}

		public static ButtonBuilder FromPrefab(GameObject prefab) {
			return new ButtonBuilder(prefab);
		}

		public ButtonBuilder Text(string text) {
			this.text = text;
			return this;
		}

		public ButtonBuilder Enabled(bool enabled) {
			this.enabled = enabled;
			return this;
		}

		public ButtonBuilder OnClick(Action onClick) {
			this.onClick = onClick;
			return this;
		}

		public IButtonHandle Build(IUIElementContainer container) {
			if (built) throw new InvalidOperationException("Button already built");
			built = true;

			GameObject obj = GameObject.Instantiate(prefab);
			UIElementNode node = new(obj);
			container.AddElement(node);

			var handle = obj.GetOrAddComponent<ButtonHandle>();
			handle.Build(text, enabled, onClick);

			return handle;
		}
	}
}
