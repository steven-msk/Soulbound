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
		public ItemStack stack { get; private set; } = null!;

		// at some point display rebinding will be necessary to avoid UI churn
		// item displays are currently not cached for stacks, all displays are recreated for each stack
		// this results in displays dissapearing for a frame or two until the object is fully visible
		// this issue will be handled during prod
		public static ItemDisplay Create(ItemStack itemStack, Func<Transform?> parentSupplier) {
			GameObject obj = Instantiate(AssetManager.Resolve<GameObject>(itemDisplay), parentSupplier.Invoke());
			ItemDisplay display = obj.GetComponent<ItemDisplay>();

			display.SetStack(itemStack);
			display.stackText = CreateStackText(display);
			display.BuildVisuals();
			return display;
		}

		private static TextMeshProUGUI CreateStackText(ItemDisplay display) {
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

			return text;
		}

		private void SetStack(ItemStack stack) {
			this.stack = stack;
			stack.onQuantityChanged += OnStackQuantityChanged;
			BuildVisuals();
		}

		private void BuildVisuals() {
			Sprite sprite = AssetManager.Resolve<Sprite>(stack.item.aspect.icon.spriteKey);
			GetComponent<Image>().sprite = sprite;
			stackText.enabled = stack.item.IsStackable();

			// the display needs to be last in rendering layer
			// setting the transform to last sibling doesnt guarantee total visibility for every layer
			// but for this stage of the game its good enough
			transform.SetAsLastSibling();
		}

		public void SetRaycastTarget(bool raycastTarget) {
			this.GetComponent<Image>().raycastTarget = raycastTarget;
		}

		public void Destroy() {
			onDestroy?.Invoke(this);
			stack.onQuantityChanged -= OnStackQuantityChanged;
			GameObject.Destroy(gameObject);
		}

		public void UpdateStackText() {
			stackText.text = stack.quantity.ToString();
		}

		[Obsolete("Destroy shouldnt be autonomously called")]
		public void OnStackQuantityChanged(int old, int @new) {
			if (@new <= 0) Destroy();
			else UpdateStackText();
		}
	}
}
