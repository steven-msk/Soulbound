namespace SoulboundEngine.UnityClient.UI.Screen {
	using SoulboundEngine.Item.Container;
	using SoulboundEngine.UnityClient.Render.Item;
	using SoulboundEngine.UnityClient.UI.UXMLBindings;
	using UnityEngine.UIElements;

	public class UXMLArmorSlotDisplay : UXMLItemSlotDisplay {
		private static readonly UXMLBinding<VisualElement> DISPLAY_ELEMENT = new("soulbound:armor_slot/item_display");
		private static readonly UXMLBinding<Label> STACK_COUNT_ELEMENT = new("soulbound:armor_slot/stack_count");
		private static readonly UXMLBinding<ProgressBar> DURABILITY_BAR_ELEMENT = new("soulbound:armor_slot/durability_bar");

		public UXMLArmorSlotDisplay(IItemSlot slot, ItemRenderManager itemRenderManager, bool interactable, bool showTooltip = true)
			: base(slot, itemRenderManager, interactable, showTooltip) {
		}

		protected override VisualElement GetDisplayElement(VisualElement root) {
			return DISPLAY_ELEMENT.Get(root);
		}

		protected override ProgressBar GetDurabilityBar(VisualElement root) {
			return DURABILITY_BAR_ELEMENT.Get(root);
		}

		protected override Label GetStackCountElement(VisualElement root) {
			return STACK_COUNT_ELEMENT.Get(root);
		}
	}
}
