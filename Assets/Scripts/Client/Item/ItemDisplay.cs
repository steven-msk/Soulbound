using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.UI;
using SoulboundBackend.Client.UI.Screens;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Client.UI.Tooltip;
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
			//UnityEngine.Debug.Assert(display != null, $"ItemDisplay component not found in item display prefab");

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
			//DestroyTooltip();
			GameObject.Destroy(gameObject);
		}

		[Obsolete]
		public void OnGrab(Transform? grabParent, bool keepWorldSpace = false) {
			//transform.SetParent(grabParent, keepWorldSpace);
			//gameObject.GetComponent<Image>().raycastTarget = false;
			//DestroyTooltip();
		}

		[Obsolete]
		public void OnRelease(Transform? releaseParent, bool keepWorldSpace = false) {
			//transform.SetParent(releaseParent, keepWorldSpace);
			//gameObject.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
			//gameObject.GetComponent<Image>().raycastTarget = true;
		}

		[Obsolete]
		public void DestroyTooltip() {
			//activeTooltip?.Hide();
			//activeTooltip = null;
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
