using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Client.UI.Tooltip;
using SoulboundBackend.Core;
using SoulboundBackend.Core.Resource;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

#nullable enable

namespace SoulboundBackend.Client.ItemSystem {
	public class ItemDisplay : MonoBehaviour {
		public event Action<ItemStack>? onDestroy;
		[SerializeField] private bool moveMode;
		[SerializeField] private Item displayedItem;
		public GameObject stackText { get; private set; } = null!;
		private ItemStack itemStack = null!;
		public ItemStack stack {
			get => itemStack;
			set {
				itemStack = value;
				gameObject.GetComponent<Image>().sprite = itemStack.item.aspect.icon.sprite;
				displayedItem = itemStack.item;
			}
		}
		public Item? item => stack?.item;
		public Tooltip? activeTooltip { get; private set; } = null;

		public static ItemDisplay Create(ItemStack itemStack, Func<Transform?> parentSupplier) {
			var prefab = ResourceManager.Get<GameObject, ResourceGroups.Prefabs>("itemDisplayPrefab");
			GameObject? obj = Instantiate(prefab, parentSupplier.Invoke());
			ItemDisplay? display = obj?.GetComponent<ItemDisplay>();
			UnityEngine.Debug.Assert(display != null, $"ItemDisplay component not found in item display prefab");

			display!.stack = itemStack;
			display.stackText = itemStack.AssignDisplay(display);
			display.transform.SetAsLastSibling();
			return display;
		}

		private void Update() {
			Vector2 mousePos = UnityEngine.Input.mousePosition;
			if (moveMode) {
				gameObject.transform.position = mousePos;
			}
			activeTooltip?.SetPosition(mousePos);
		}

		public void Destroy() {
			onDestroy?.Invoke(itemStack);
			DestroyTooltip();
			GameObject.Destroy(gameObject);
		}

		public void ShowTooltip(Vector2 position, Transform? parent) {
			activeTooltip = itemStack.item.RenderTooltip(position, this.transform);
			activeTooltip?.SetParent(parent!, true);
		}

		public void OnGrab(Transform? grabParent, bool keepWorldSpace = false) {
			moveMode = true;
			transform.SetParent(grabParent, keepWorldSpace);
			gameObject.GetComponent<Image>().raycastTarget = false;
			DestroyTooltip();
		}

		public void OnRelease(Transform? releaseParent, bool keepWorldSpace = false) {
			moveMode = false;
			transform.SetParent(releaseParent, keepWorldSpace);
			gameObject.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
			gameObject.GetComponent<Image>().raycastTarget = true;
		}

		public void DestroyTooltip() {
			activeTooltip?.Hide();
			activeTooltip = null;
		}

		public void UpdateStackText() {
			TextMeshProUGUI? stackText = this.stackText?.GetComponent<TextMeshProUGUI>();
			stackText!.text = itemStack.quantity.ToString();
		}

		public void OnStackQuantityChanged(int old, int @new) {
			if (@new <= 0) {
			   this.Destroy();
			}
			if (stackText != null && @new > 0) {
				this.UpdateStackText();
			}
		}
	}
}