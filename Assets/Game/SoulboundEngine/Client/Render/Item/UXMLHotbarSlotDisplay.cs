using SoulboundEngine.Client.Item.Container;
using SoulboundEngine.Client.UI.UXMLBindings;
using SoulboundEngine.Core.Registry;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.Render.Item {
	public class UXMLHotbarSlotDisplay : UXMLItemSlotDisplay {
		private static readonly Identifier DISPLAY_AREA_ELEMENT = Identifier.Of("soulbound:hotbar_slot/display_area");
		private static readonly Identifier SLOT_INDEX_ELEMENT = Identifier.Of("soulbound:hotbar_slot/slot_index");
		private static readonly Identifier DURABILITY_BAR_ELEMENT = Identifier.Of("soulbound:hotbar_slot/durability_bar");
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

		public override void OnBind(VisualElement root) {
			base.OnBind(root);
			root.Get<Label>(SLOT_INDEX_ELEMENT).text = (this.slot.GetIndex() + 1).ToString();
		}

		protected override Identifier GetDurabilityBarId() => DURABILITY_BAR_ELEMENT;

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
			VisualElement displayArea = this.root.Get<VisualElement>(DISPLAY_AREA_ELEMENT);
			displayArea.style.borderTopColor = borders[0];
			displayArea.style.borderRightColor = borders[1];
			displayArea.style.borderBottomColor = borders[2];
			displayArea.style.borderLeftColor = borders[3];
		}

		private static Color GetColorFromHex(string hex) {
			if (!ColorUtility.TryParseHtmlString(hex, out Color color)) {
				throw new ArgumentException("Unknown color hex: " + hex);
			}
			return color;
		}
	}
}
