using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace SoulboundBackend.Client.UI {
	public sealed class PlayerInventoryUIBuilder : IUIElementHandleBuilder<PlayerInventoryHandle> {
		private readonly PlayerInventory inventory;

		public PlayerInventoryUIBuilder(PlayerInventory inventory) {
			this.inventory = inventory;
		}

		public PlayerInventoryHandle Build(IUIElementContainer container) {
			GameObject obj = new("Player Inventory", typeof(RectTransform));
			RectTransform rect = obj.GetComponent<RectTransform>();
			rect.pivot = rect.anchorMin = rect.anchorMax = Vector2.zero;

			GridLayoutGroup layoutGroup = obj.AddComponent<GridLayoutGroup>();
			layoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
			layoutGroup.constraintCount = 9;
			layoutGroup.cellSize = new Vector2(32f, 32f);
			ContentSizeFitter sizeFitter = obj.AddComponent<ContentSizeFitter>();
			sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
			sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

			foreach (var slot in inventory.GetAllSlots()) {
				GameObject slotObj = new("Inventory Slot", typeof(RectTransform));
				InventorySlotHandle slotHandle = slotObj.AddComponent<InventorySlotHandle>();

				LayoutElement layoutElement = slotObj.AddComponent<LayoutElement>();
				layoutElement.preferredHeight = 32f;
				layoutElement.preferredWidth = 32f;

				slotHandle.Init(slot.GetStack() /* , container */);
				slotObj.transform.SetParent(obj.transform, false);
			}

			PlayerInventoryHandle handle = obj.AddComponent<PlayerInventoryHandle>();
			container.AddElement(new UIElementNode(obj));
			return handle;
		}
	}
}
