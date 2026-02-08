using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Core.AssetManagement;
using SoulboundBackend.Core.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace SoulboundBackend.Client.UI {
	public sealed class PlayerInventoryUIBuilder : IUIElementHandleBuilder<IItemContainerHandle> {
		private static readonly AssetKey slotKey = new("Slot");
		private readonly PlayerInventory inventory;
		private IItemSlotEventCallbacks handleCallbacks;

		public PlayerInventoryUIBuilder(PlayerInventory inventory) {
			this.inventory = inventory;
		}

		public IItemContainerHandle Build(IUIElementContainer container) {
			GameObject obj = new("Player Inventory", typeof(RectTransform));
			RectTransform rect = obj.GetComponent<RectTransform>();
			rect.pivot = rect.anchorMin = rect.anchorMax = Vector2.zero;

			GridLayoutGroup layoutGroup = obj.AddComponent<GridLayoutGroup>();
			layoutGroup.spacing = new Vector2(5f, 5f);
			layoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
			layoutGroup.constraintCount = 9;
			layoutGroup.cellSize = new Vector2(32f, 32f);

			ContentSizeFitter sizeFitter = obj.AddComponent<ContentSizeFitter>();
			sizeFitter.verticalFit = sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

			PlayerInventoryHandle inventoryHandle = obj.AddComponent<PlayerInventoryHandle>();
			handleCallbacks = (IItemSlotEventCallbacks)inventoryHandle;
			inventoryHandle.Init(inventory);

			List<InventorySlotHandle> popupHandles = new();
			foreach (var slot in inventory.GetPopup()) {
				GameObject slotObj = CreateSlotObj(slot, out var handle);
				popupHandles.Add(handle);
				slotObj.transform.SetParent(obj.transform, false);
			}

			InventorySlot[] hotbar = inventory.GetHotbar();
			List<InventorySlotHandle> hotbaHandles = new();
			for (int i = 0; i < hotbar.Length; i++) {
				GameObject slotObj = CreateHotbarSlotObj(hotbar[i], i + 1, out var handle);
				hotbaHandles.Add(handle);
				slotObj.transform.SetParent(obj.transform, false);
			}

			container.AddElement(new UIElementNode(obj));

			inventory.togglePopup += () => {
				foreach (var popupHandle in popupHandles) {
					popupHandle.ToggleVisibility();
				}
			};

			inventory.TogglePopup();
			return inventoryHandle;
		}


		private GameObject CreateSlotObj(IItemSlot slot, out InventorySlotHandle handle) {
			GameObject obj = GameObject.Instantiate(AssetManager.Resolve<GameObject>(slotKey));
			handle = obj.AddComponent<InventorySlotHandle>();
			handle.Init(slot);
			handle.pointerDown += handleCallbacks.OnPointerDown;
			handle.pointerUp += handleCallbacks.OnPointerUp;
			handle.pointerEnter += handleCallbacks.OnPointerEnter;
			handle.pointerExit += handleCallbacks.OnPointerExit;
			return obj;
		}

		private GameObject CreateHotbarSlotObj(IItemSlot slot, int index, out InventorySlotHandle handle) {
			GameObject obj = CreateSlotObj(slot, out handle);
			GameObject textObj = new("Hotbar Slot Number", typeof(RectTransform));

			ContentSizeFitter sizeFitter = textObj.AddComponent<ContentSizeFitter>();
			sizeFitter.horizontalFit = sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

			TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
			text.fontSize = 7f;
			text.text = index.ToString();

			RectTransform rect = textObj.GetComponent<RectTransform>();
			rect.pivot = rect.anchorMax = rect.anchorMin = new Vector2(0.5f, 1f);

			textObj.transform.SetParent(obj.transform, false);
			return obj;
		}
	}
}
