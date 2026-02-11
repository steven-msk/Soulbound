using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Common;
using SoulboundBackend.Core.AssetManagement;
using SoulboundBackend.Core.Resource;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.UI;

namespace SoulboundBackend.Client.UI {
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

		public IItemContainerHandle BuildInventory(IWorldSessionScreenObject sessionScreen) {
			InventoryHandle inventory = CreateContainerObject<InventoryHandle>(
				"Inventory", Inventory.COLUMNS
			);
			RectTransform rect = inventory.GetComponent<RectTransform>();
			rect.pivot = rect.anchorMin = rect.anchorMax = Vector2.zero;

			List<InventorySlotHandle> slotHandles = new();
			foreach (var slot in this.inventory.GetAllSlots()) {
				GameObject slotObj = CreateSlotObj(slot, inventory, out InventorySlotHandle handle);
				slotHandles.Add(handle);
				slotObj.transform.SetParent(inventory.transform, false);
			}

			this.inventory.toggle += () => {
				foreach (var handle in slotHandles) {
					handle.ToggleVisibility();
				}
			};

			inventory.Init(this.inventory, sessionScreen);
			UIItemContainerNode node = new(inventory.gameObject, this.inventory, inventory);
			sessionScreen.AddItemContainer(node);

			this.inventory.Toggle();
			sessionScreen.AddElement(new UIElementNode(inventory.gameObject));
			return inventory;
		}

		public IItemContainerHandle BuildHotbar(IWorldSessionScreenObject sessionScreen) {
			InventoryHandle hotbar = CreateContainerObject<InventoryHandle>(
				"Hotbar", Hotbar.COLUMNS
			);
			RectTransform rect = hotbar.GetComponent<RectTransform>();
			rect.pivot = rect.anchorMin = rect.anchorMax = Vector2.zero;

			List<IItemSlot>.Enumerator enumerator = this.hotbar.GetAllSlots().ToList().GetEnumerator();
			int i = 0;
			while (enumerator.MoveNext()) {
				GameObject slotObj = CreateHotbarSlotObj(enumerator.Current, hotbar, i, out InventorySlotHandle handle);
				slotObj.transform.SetParent(hotbar.transform, false);
				i++;
			}

			hotbar.Init(this.hotbar, sessionScreen);
			UIItemContainerNode node = new(hotbar.gameObject, this.hotbar, hotbar);
			sessionScreen.AddItemContainer(node);

			sessionScreen.AddElement(new UIElementNode(hotbar.gameObject));
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

		private GameObject CreateSlotObj(IItemSlot slot, IItemSlotHandleCallbacks handleCallbacks, out InventorySlotHandle handle) {
			GameObject obj = GameObject.Instantiate(AssetManager.Resolve<GameObject>(slotKey));
			handle = obj.AddComponent<InventorySlotHandle>();
			handle.Init(slot);
			handle.pointerDown += handleCallbacks.OnPointerDown;
			handle.pointerUp += handleCallbacks.OnPointerUp;
			handle.pointerEnter += handleCallbacks.OnPointerEnter;
			handle.pointerExit += handleCallbacks.OnPointerExit;
			return obj;
		}

		private GameObject CreateHotbarSlotObj(IItemSlot slot, IItemSlotHandleCallbacks handleCallbacks, int index, out InventorySlotHandle handle) {
			GameObject obj = CreateSlotObj(slot, handleCallbacks, out handle);
			GameObject textObj = new("Hotbar Slot Number", typeof(RectTransform));

			ContentSizeFitter sizeFitter = textObj.AddComponent<ContentSizeFitter>();
			sizeFitter.horizontalFit = sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

			TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
			text.fontSize = HOTBAR_NUMBER_FONT_SIZE;
			text.text = index.ToString();

			RectTransform rect = textObj.GetComponent<RectTransform>();
			rect.pivot = rect.anchorMax = rect.anchorMin = new Vector2(0.5f, 1f);

			textObj.transform.SetParent(obj.transform, false);
			return obj;
		}
	}
}
