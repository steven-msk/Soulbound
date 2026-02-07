using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Core.AssetManagement;
using SoulboundBackend.Core.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace SoulboundBackend.Client.UI {
	public sealed class PlayerInventoryUIBuilder : IUIElementHandleBuilder<IPlayerInventoryHandle> {
		private static readonly GameObject slotPrefab = AssetManager.Resolve<GameObject>(new AssetKey("Slot"));
		private readonly PlayerInventory inventory;

		public PlayerInventoryUIBuilder(PlayerInventory inventory) {
			this.inventory = inventory;
		}

		public IPlayerInventoryHandle Build(IUIElementContainer container) {
			GameObject obj = new("Player Inventory", typeof(RectTransform));
			RectTransform rect = obj.GetComponent<RectTransform>();
			rect.pivot = rect.anchorMin = rect.anchorMax = Vector2.zero;

			GridLayoutGroup layoutGroup = obj.AddComponent<GridLayoutGroup>();
			layoutGroup.spacing = new Vector2(5f, 5f);
			layoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
			layoutGroup.constraintCount = 9;
			layoutGroup.cellSize = new Vector2(32f, 32f);

			ContentSizeFitter sizeFitter = obj.AddComponent<ContentSizeFitter>();
			sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
			sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

			foreach (var slot in inventory.GetAllSlots()) {
				GameObject slotObj = CreateSlotObj(slot);
				slotObj.transform.SetParent(obj.transform, false);
			}

			IPlayerInventoryHandle handle = obj.AddComponent<PlayerInventoryHandle>();
			container.AddElement(new UIElementNode(obj));
			return handle;
		}


		private GameObject CreateSlotObj(IItemSlot slot) {
			GameObject slotObj = GameObject.Instantiate(slotPrefab);
			InventorySlotHandle slotHandle = slotObj.AddComponent<InventorySlotHandle>();
			slotHandle.Init(slot.GetStack() /* , container */);
			return slotObj;
		}
	}
}
