using SoulboundEngine.Client.ItemSystem.Container;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.Render.Item {
	public sealed class HotbarSlotHandle : UIToolkitItemSlotHandle {
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

		public HotbarSlotHandle(IItemSlot slot, ItemRenderManager itemRenderManager) 
			: base(slot, itemRenderManager) {
		}

		public override void OnBind(VisualElement root) {
			base.OnBind(root);
			root.Q<Label>("SlotIndex").text = (this.slot.GetIndex() + 1).ToString();
		}

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
			VisualElement displayArea = this.root.Q<VisualElement>("DisplayArea");
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
