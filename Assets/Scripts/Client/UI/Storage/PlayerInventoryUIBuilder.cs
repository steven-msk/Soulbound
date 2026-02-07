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

			foreach (var slot in inventory.GetPopup()) {
				GameObject slotObj = CreateSlotObj(slot);
				slotObj.transform.SetParent(obj.transform, false);
			}

			InventorySlot[] hotbar = inventory.GetHotbar();
			for (int i = 0; i < hotbar.Length; i++) {
				GameObject slotObj = CreateHotbarSlotObj(hotbar[i], i + 1);
				slotObj.transform.SetParent(obj.transform, false);
			}

			IPlayerInventoryHandle handle = obj.AddComponent<PlayerInventoryHandle>();
			container.AddElement(new UIElementNode(obj));
			return handle;
		}


		private GameObject CreateSlotObj(IItemSlot slot) {
			GameObject obj = GameObject.Instantiate(slotPrefab);
			InventorySlotHandle slotHandle = obj.AddComponent<InventorySlotHandle>();
			slotHandle.Init(slot.GetStack() /* , container */);
			return obj;
		}

		private GameObject CreateHotbarSlotObj(IItemSlot slot, int index) {
			GameObject obj = CreateSlotObj(slot);
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
