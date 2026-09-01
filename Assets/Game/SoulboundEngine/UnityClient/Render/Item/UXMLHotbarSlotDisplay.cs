namespace SoulboundEngine.UnityClient.Render.Item {
	using SoulboundEngine.Item.Container;
	using SoulboundEngine.UnityClient.UI.UXMLBindings;
	using System;
	using UnityEngine;
	using UnityEngine.UIElements;

	public class UXMLHotbarSlotDisplay : UXMLItemSlotDisplay {
		private static readonly UXMLBinding<VisualElement> DISPLAY_AREA_ELEMENT = new("soulbound:hotbar_slot/display_area");
		private static readonly UXMLBinding<Label> SLOT_INDEX_ELEMENT = new("soulbound:hotbar_slot/slot_index");
		private static readonly UXMLBinding<ProgressBar> DURABILITY_BAR_ELEMENT = new("soulbound:hotbar_slot/durability_bar");
		private bool isMainSlot;
		private static readonly Color[] mainSlotBorders = {
			Color.white, Color.white, Color.white, Color.white
		};
		private static readonly Color[] defaultBorders = {
			GetColorFromHex("#808080"),		// top
			GetColorFromHex("#666666"),		// right
			GetColorFromHex("#666666"),		// bottom
			GetColorFromHex("#808080")		// left
		};

		public UXMLHotbarSlotDisplay(IItemSlot slot, ItemRenderManager itemRenderManager, bool interactable) 
			: base(slot, itemRenderManager, interactable) {
		}

		protected override void Prepare() {
			SLOT_INDEX_ELEMENT.Get(this.root).text = (this.slot.GetIndex() + 1).ToString();
		}

		protected override ProgressBar GetDurabilityBar(VisualElement root) => DURABILITY_BAR_ELEMENT.Get(root);

		public override void SetAsMainSlot() {
			if (this.isMainSlot) return;
			this.isMainSlot = true;
			this.SetBorders(mainSlotBorders);
		}

		public override void UnsetMainSlot() {
			if (!this.isMainSlot) return;
			this.isMainSlot = false;
			this.SetBorders(defaultBorders);
		}

		private void SetBorders(Color[] borders) {
			VisualElement displayArea = DISPLAY_AREA_ELEMENT.Get(this.root);
			displayArea.style.borderTopColor = borders[0];
			displayArea.style.borderRightColor = borders[1];
			displayArea.style.borderBottomColor = borders[2];
			displayArea.style.borderLeftColor = borders[3];
		}

		private static Color GetColorFromHex(string hex) {
			return !ColorUtility.TryParseHtmlString(hex, out Color color) ? throw new ArgumentException("Unknown color hex: " + hex) : color;
		}
	}
}
