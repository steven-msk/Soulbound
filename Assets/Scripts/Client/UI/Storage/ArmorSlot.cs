using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.Stats;
using SoulboundBackend.Core;
using UnityEngine;

#nullable enable

namespace SoulboundBackend.Client.UI.Storage {
	public class ArmorSlot : EquipmentSlot, IItemSlot {
		[SerializeField] private ArmorType armorType;
		public ArmorType AcceptedType => armorType;

		[SerializeField] private GameObject overlay;
		public override int index { get; set; }
		public override bool showTooltip { get; set; } = true;

		public override IItemContainer container => this.GetComponentInParent<InventoryController>();

		bool IItemSlot.Handshake(ItemDisplay? grabbedItem, SlotInteractionMode interactionMode) {
			if (interactionMode == SlotInteractionMode.Drag) {
				return false;
			}
			PlayerStats playerStats = Soulbound.instance.GetPlayerInstance().Stats;

			if (grabbedItem?.item is ArmorItem armor && armor.armorType == this.AcceptedType) {
				if (this.HasItem) {
					//this.CastDisplayed()!.OnUnequipped(playerStats);
				}
				//armor.OnEquip(playerStats);
				overlay.SetActive(false);
				return true;
			}
			if (grabbedItem == null && this.HasItem) {
				//this.CastDisplayed()?.OnUnequipped(playerStats);
				overlay.SetActive(true);
				return true;
			}
			return false;
		}

		private ArmorItem? CastDisplayed() => this.stack?.item as ArmorItem;

		public override void Deserialize(SerializedItemSlot serialized) {
			ItemSlotDeserializer.Deserialize(this, serialized);
			this.overlay.SetActive(!this.HasItem);
		}
	}
}
