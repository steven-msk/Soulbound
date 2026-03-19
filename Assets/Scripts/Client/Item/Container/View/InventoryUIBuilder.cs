using SoulboundBackend.Client.ItemSystem.Container;
using SoulboundBackend.Client.UI;
using SoulboundBackend.Common;
using SoulboundBackend.Core.Assets;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoulboundBackend.Client.ItemSystem.Container.View {
	public sealed class InventoryUIBuilder {
		const float GRID_SPACING = 5f;
		const float CELL_SIZE = 32f;
		const float HOTBAR_NUMBER_FONT_SIZE = 7f;

		private static readonly AssetKey slotKey = new("Slot");
		private readonly Inventory inventory;
		private readonly Hotbar hotbar;

		public InventoryUIBuilder(Inventory inventory, Hotbar hotbar) {
			this.inventory = inventory;
			this.hotbar = hotbar;
		}

		public void Build(IItemContainerScreenScope screenScope, out IItemContainerHandle inventory, out IItemContainerHandle hotbar) {
			inventory = BuildInventory(screenScope);
			hotbar = BuildHotbar(screenScope);

			this.inventory.toggle += ((HotbarHandle)hotbar).ToggleFadedLayout;
			this.inventory.Toggle();
		}

		private InventoryHandle BuildInventory(IItemContainerScreenScope screenScope) {
			InventoryHandle inventory = CreateContainerObject<InventoryHandle>(
				"Inventory", Inventory.COLUMNS
			);
			RectTransform rect = inventory.GetComponent<RectTransform>();
			rect.pivot = rect.anchorMin = rect.anchorMax = Vector2.zero;

			List<ItemSlotHandle> slotHandles = new();
			foreach (var slotIndex in this.inventory.GetAllSlots()) {
				IItemSlot slot = this.inventory.GetSlot(slotIndex);
				GameObject slotObj = CreateSlotObj(slot, inventory, out ItemSlotHandle handle);
				slotHandles.Add(handle);
				slotObj.transform.SetParent(inventory.transform, false);
			}

			this.inventory.toggle += () => {
				foreach (var handle in slotHandles) {
					handle.ToggleVisibility();
				}
			};

			inventory.Init(this.inventory, screenScope);
			UIItemContainerNode node = new(inventory.gameObject, this.inventory, inventory);
			screenScope.AddItemContainer(node);

			screenScope.AddElement(new UIElementNode(inventory.gameObject));
			return inventory;
		}

		private InventoryHandle BuildHotbar(IItemContainerScreenScope screenScope) {
			HotbarHandle hotbar = CreateContainerObject<HotbarHandle>(
				"Hotbar", Hotbar.SLOT_COUNT
			);
			RectTransform rect = hotbar.GetComponent<RectTransform>();
			rect.pivot = rect.anchorMin = rect.anchorMax = Vector2.zero;

			List<int>.Enumerator enumerator = this.hotbar.GetAllSlots().ToList().GetEnumerator();
			HotbarSlotHandle[] handles = new HotbarSlotHandle[this.hotbar.GetSlotCount()];
			int slotIndex = 0;
			while (enumerator.MoveNext()) {
				IItemSlot slot = this.hotbar.GetSlot(enumerator.Current);
				GameObject slotObj = CreateHotbarSlotObj(slot, hotbar, slotIndex, out HotbarSlotHandle handle);
				handles[slotIndex++] = handle;
				slotObj.transform.SetParent(hotbar.transform, false);
			}

			hotbar.Init(this.hotbar, screenScope, handles);
			UIItemContainerNode node = new(hotbar.gameObject, this.hotbar, hotbar);
			screenScope.AddItemContainer(node);

			screenScope.AddElement(new UIElementNode(hotbar.gameObject));
			return hotbar;
		}

		[PROTOTYPICAL]
		public void FixInventoryPosition(IItemContainerHandle inventory, IItemContainerHandle hotbar) {
			InventoryHandle inventoryHandle = (InventoryHandle)inventory;
			InventoryHandle hotbarHandle = (InventoryHandle)hotbar;
			RectTransform inventoryRect = inventoryHandle.GetComponent<RectTransform>();
			RectTransform hotbarRect = hotbarHandle.GetComponent<RectTransform>();

			const float fixedSpacing = CELL_SIZE + GRID_SPACING;
			float fixedY = fixedSpacing - inventoryRect.anchoredPosition.y;		// fixedY is above current pos by fixedSpacing units
			inventoryRect.anchoredPosition = hotbarRect.anchoredPosition;		// align with hotbar
			inventoryRect.anchoredPosition += new Vector2(0f, fixedY);
		}

		private THandle CreateContainerObject<THandle>(string name, int columns)
				where THandle : UnityEngine.Component, IItemContainerHandle, new() {
			GameObject obj = new(name, typeof(RectTransform));

			GridLayoutGroup layoutGroup = obj.AddComponent<GridLayoutGroup>();
			layoutGroup.spacing = new Vector2(GRID_SPACING, GRID_SPACING);
			layoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
			layoutGroup.constraintCount = columns;
			layoutGroup.cellSize = new Vector2(CELL_SIZE, CELL_SIZE);

			ContentSizeFitter sizeFitter = obj.AddComponent<ContentSizeFitter>();
			sizeFitter.verticalFit = sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

			THandle containerHandle = obj.AddComponent<THandle>();
			return containerHandle;
		}

		private GameObject CreateSlotObj(IItemSlot slot, IItemSlotHandleCallbacks handleCallbacks, out ItemSlotHandle handle) {
			GameObject obj = GameObject.Instantiate(AssetManager.Resolve<GameObject>(slotKey));
			handle = obj.AddComponent<ItemSlotHandle>();
			InitSlotHandle(handle, slot, handleCallbacks);
			return obj;
		}

		private GameObject CreateHotbarSlotObj(IItemSlot slot, IItemSlotHandleCallbacks handleCallbacks, int index, out HotbarSlotHandle handle) {
			GameObject obj = GameObject.Instantiate(AssetManager.Resolve<GameObject>(slotKey));
			handle = obj.AddComponent<HotbarSlotHandle>();
			InitSlotHandle(handle, slot, handleCallbacks);			

			GameObject textObj = new("Hotbar Slot Number", typeof(RectTransform));
			ContentSizeFitter sizeFitter = textObj.AddComponent<ContentSizeFitter>();
			sizeFitter.horizontalFit = sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

			TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
			text.fontSize = HOTBAR_NUMBER_FONT_SIZE;
			text.text = (index + 1).ToString();

			RectTransform rect = textObj.GetComponent<RectTransform>();
			rect.pivot = rect.anchorMax = rect.anchorMin = new Vector2(0.5f, 1f);

			textObj.transform.SetParent(obj.transform, false);
			return obj;
		}

		private void InitSlotHandle(ItemSlotHandle handle, IItemSlot slot, IItemSlotHandleCallbacks handleCallbacks) {
			handle.Init(slot);
			handle.pointerDown += handleCallbacks.OnPointerDown;
			handle.pointerUp += handleCallbacks.OnPointerUp;
			handle.pointerEnter += handleCallbacks.OnPointerEnter;
			handle.pointerExit += handleCallbacks.OnPointerExit;
		}
	}
}
