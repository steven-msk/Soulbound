using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Client.UI.Tooltip;
using SoulboundBackend.Core;
using SoulboundBackend.Core.Resource;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

#nullable enable

namespace SoulboundBackend.Client.ItemSystem {
	public class ItemDisplay : MonoBehaviour {
		private PlayerController player;
		[SerializeField] private bool moveMode;
		[SerializeField] private Item displayedItem;
		private ItemStack itemStack;
		public ItemStack ItemStack {
			get => itemStack;
			set {
				itemStack = value;
				gameObject.GetComponent<Image>().sprite = itemStack.item.aspect.icon.sprite;
				displayedItem = itemStack.item;
			}
		}
		public TooltipRenderer tooltipRenderer { get; private set; }
		public Item? DisplayedItem => ItemStack?.item;
		public Tooltip? activeTooltip { get; private set; } = null;

		public static ItemDisplay Create(ItemStack itemStack, IItemSlot slot) {
			ItemDisplay display = Create(itemStack, () => slot.GameObject.transform);
			slot.AttachItemDisplay(display);
			return display;
		}

		public static ItemDisplay Create(Item item, int quantity, IItemSlot slot) {
			return Create(new ItemStack(item, quantity), slot);
		}

		public static ItemDisplay Create(ItemStack itemStack, Func<Transform?> parentSupplier) {
			GameObject? obj = Instantiate(ResourceManager.Get<GameObject, ResourceGroups.Prefabs>("itemDisplayPrefab"), parentSupplier.Invoke());
			ItemDisplay? display = obj?.GetComponent<ItemDisplay>();
			UnityEngine.Debug.Assert(display != null, $"ItemDisplay component not found in item display prefab");
			display!.ItemStack = itemStack;
			itemStack.AssignDisplay(display);
			display.tooltipRenderer = new TooltipRenderer(TooltipNodeStylePresets.PresetProvider());
			display.transform.SetAsLastSibling();
			display.player = Soulbound.instance.GetActiveLevel()!.Player;
			return display;
		}

		private void Update() {
			if (moveMode) {
				gameObject.transform.position = player.InputHandler.MouseScreenPosition;
			}
			activeTooltip?.SetPosition(player.InputHandler.MouseScreenPosition);
		}

		public void Destroy() {
			if (ItemStack == player.MainHandStack) {
				player.SetMainHandItem(null);
			}
			DestroyTooltip();
			GameObject.Destroy(gameObject);
		}

		public void ShowTooltip(Vector2 position) {
			activeTooltip = itemStack.item.RenderTooltip(position, this.transform);
			activeTooltip?.SetParent(player.Inventory.transform, true);
		}

		public void OnGrab() {
			moveMode = true;
			gameObject.GetComponent<Image>().raycastTarget = false;
			DestroyTooltip();
		}

		public void OnRelease() {
			moveMode = false;
			gameObject.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
			gameObject.GetComponent<Image>().raycastTarget = true;
		}

		public void DestroyTooltip() {
			activeTooltip?.Hide();
			activeTooltip = null;
		}
	}
}