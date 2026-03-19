using UnityEngine;
using UnityEngine.UI;

namespace SoulboundBackend.Client.ItemSystem.Container.View {
	public sealed class HotbarSlotHandle : ItemSlotHandle {
		static readonly Color fadedColor = new(0.6f, 0.6f, 0.6f);
		static readonly Color normalColor = new(1f, 1f, 1f);
		private bool fadedLayout;

		// keep in mind that GridLayoutGroup controls size and position
		// any changes related to those need to be made with this in mind
		// for simplicity and prototyping reasons, the only visual change
		// will consist in a white/gray color changed when the faded layout
		// or main slot layout is applied

		// one fix would be to use shaders, but they should be used
		// to change cosmetic visuals, reather than structural visuals
		// instead, a better way here is to use parenting
		// to the advantage that the layout isnt applied to the children of this object,
		// only the parent

		public void ApplyFadedLayout(bool fadedLayout) {
			this.fadedLayout = fadedLayout;

			Image image = GetComponent<Image>();
			image.color = fadedLayout ? fadedColor : normalColor;
		}

		public void SetMainSlotLayout() {
			Image image = GetComponent<Image>();
			image.color = normalColor;
		}

		public void RemoveMainSlotLayout() {
			ApplyFadedLayout(fadedLayout);
		}
	}
}
