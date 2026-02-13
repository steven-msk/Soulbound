using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.UI;
using SoulboundBackend.Client.UI.Screens;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Core;
using SoulboundBackend.Core.AssetManagement;
using SoulboundBackend.Core.Resource;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

#nullable enable

namespace SoulboundBackend.Client.ItemSystem {
	public class ItemDisplay : MonoBehaviour {
		[Obsolete] private static readonly AssetKey itemDisplay = new("itemDisplayPrefab");
		[Obsolete] private static readonly AssetKey stackNumber = new("stackNumberPrefab");
		public event Action<ItemDisplay>? onDestroy;
		public TextMeshProUGUI stackText { get; private set; } = null!;
		private ItemStack itemStack = null!;
		public ItemStack stack {
			get => itemStack;
			set {
				itemStack = value;

				// prototypical - will be implemented correctly when visuals system is improved

				var sprite = AssetManager.Resolve<Sprite>(itemStack.item.aspect.icon.spriteKey);
				gameObject.GetComponent<Image>().sprite = sprite;
			}
		}
		public Item item => stack.item;

		public static ItemDisplay Create(ItemStack itemStack, Func<Transform?> parentSupplier) {
			GameObject obj = Instantiate(AssetManager.Resolve<GameObject>(itemDisplay), parentSupplier.Invoke());
			ItemDisplay display = obj.GetComponent<ItemDisplay>();

			itemStack.onQuantityChanged += display.OnStackQuantityChanged;
			display.stack = itemStack;
			display.stackText = GetText(display);
			display.transform.SetAsLastSibling();
			return display;
		}

		private static TextMeshProUGUI GetText(ItemDisplay display) {
			var prefab = AssetManager.Resolve<GameObject>(stackNumber);
			GameObject? obj = GameObject.Instantiate(prefab, display.transform);

			TextMeshProUGUI? text = obj.GetComponent<TextMeshProUGUI>();
			text!.autoSizeTextContainer = true;
			text.text = display.stack.quantity.ToString();
			text.color = Color.white;

			RectTransform rect = obj.GetComponent<RectTransform>();
			rect.pivot = new Vector2(1f, 0f);
			rect.anchorMax = new Vector2(0.9375f, 0.0625f);
			rect.anchorMin = rect.anchorMax;
			rect.anchoredPosition = Vector2.zero;
			text.rectTransform.sizeDelta = text.GetPreferredValues(Mathf.Infinity, Mathf.Infinity);
			rect.anchoredPosition = new(Mathf.Max(-4, rect.sizeDelta.x - 19.14f), rect.anchoredPosition.y);

			text.enabled = display.item.IsStackable;
			return text;
		}

		public void SetRaycastTarget(bool raycastTarget) {
			this.GetComponent<Image>().raycastTarget = raycastTarget;
		}

		public void Destroy() {
			onDestroy?.Invoke(this);
			itemStack.onQuantityChanged -= OnStackQuantityChanged;
			GameObject.Destroy(gameObject);
		}

		public void UpdateStackText() {
			stackText.text = itemStack.quantity.ToString();
		}

		public void OnStackQuantityChanged(int old, int @new) {
			if (@new <= 0) Destroy();
			else UpdateStackText();
		}
	}
}
