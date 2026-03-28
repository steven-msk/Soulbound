using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#nullable enable

namespace SoulboundEngine.Client.ItemSystem.View {
	[RequireComponent(typeof(RectTransform))]
	public class UIItemDisplayView : MonoBehaviour {
		public event Action? onShouldBeDestroyed;
		private TextMeshProUGUI stackText = null!;
		private Image itemImage = null!;
		private RectTransform rect = null!;
		private ItemStack? itemStack;

		public void Init(TextMeshProUGUI stackText, Image itemImage) {
			this.stackText = stackText;
			this.itemImage = itemImage;
			rect = GetComponent<RectTransform>();
		}

		public void SetStack(ItemStack? itemStack) {
			ItemStack? oldStack = this.itemStack;
			this.itemStack = itemStack;
			UpdateVisuals(oldStack, this.itemStack);
		}

		public void SetPosition(Vector2 position) {
			// position param is in screen space coordinates
			// rect.anchoredPosition cannot be used in this case
			rect.transform.position = position;
		}

		public void SetParent(RectTransform parent) {
			rect.SetParent(parent, false);
		}

		public void Destroy() => GameObject.Destroy(gameObject);

		private void UpdateVisuals(ItemStack? oldStack, ItemStack? newStack) {
			if (oldStack != null) {
				oldStack.onQuantityChanged -= OnStackQuantityChanged;
			}

			if (newStack != null) {
				newStack.onQuantityChanged += OnStackQuantityChanged;

				// TODO: rework visual render approach for UI item displays
				//Sprite sprite = AssetManager.Resolve<Sprite>(newStack.item.aspect.icon.spriteKey);
				//itemImage.sprite = sprite;

				stackText.enabled = newStack.item.IsStackable();

				// TODO: no guarantee on full visibility for UI item displays
				transform.SetAsLastSibling();
			} else {
				onShouldBeDestroyed?.Invoke();
			}
		}

		private void OnStackQuantityChanged(int old, int @new) {
			if (@new <= 0) onShouldBeDestroyed?.Invoke();

			stackText.text = @new.ToString();
		}

		private void OnDestroy() {
			if (itemStack != null) {
				itemStack.onQuantityChanged -= OnStackQuantityChanged;
			}
		}
	}
}
