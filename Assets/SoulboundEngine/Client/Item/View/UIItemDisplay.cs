using SoulboundEngine.Client.ItemSystem;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Logger = SoulboundEngine.Client.Debug.Logging.Logger;

#nullable enable

namespace SoulboundEngine.Client.ItemSystem.View {
	public class UIItemDisplay : IItemDisplay {
		const float IMAGE_SIZE = 32f;
		const float STACK_TEXT_SIZE = 8f;

		public event Action? onDestroyed;
		private ItemStack? itemStack;
		private readonly UIItemDisplayView displayView;
		private bool destroyed = false;

		public UIItemDisplay(RectTransform parent, ItemStack itemStack) {
			this.itemStack = itemStack;

			displayView = CreateView(itemStack, parent);
			displayView.onShouldBeDestroyed += Destroy;
		}

		public ItemStack? GetStack() => itemStack;
		public void SetStack(ItemStack? itemStack) {
			if (IsDestroyed()) {
				Logger.LogError("Cannot set UI item display stack: object already destroyed");
				return;
			}
			this.itemStack = itemStack;
			displayView.SetStack(itemStack);
		}

		public void SetPosition(Vector2 position) => displayView.SetPosition(position);
		public void SetParent(RectTransform parent) => displayView.SetParent(parent);

		public bool IsDestroyed() => destroyed;

		public void Destroy() {
			onDestroyed?.Invoke();
			destroyed = true;
			displayView.Destroy();
		}

		private static UIItemDisplayView CreateView(ItemStack itemStack, RectTransform parent) {
			GameObject obj = new("UI Item Display", typeof(RectTransform));
			obj.transform.SetParent(parent, false);

			RectTransform rect = obj.GetComponent<RectTransform>();
			rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
			rect.sizeDelta = new Vector2(IMAGE_SIZE, IMAGE_SIZE);
			rect.anchoredPosition = Vector2.zero;

			Image itemImage = obj.AddComponent<Image>();
			itemImage.raycastTarget = false;
			TextMeshProUGUI stackText = CreateStackText(itemStack, rect);
			
			UIItemDisplayView displayView = obj.AddComponent<UIItemDisplayView>();
			displayView.Init(stackText, itemImage);
			displayView.SetStack(itemStack);

			return displayView;
		}

		private static TextMeshProUGUI CreateStackText(ItemStack stack, RectTransform displayParent) {
			GameObject obj = new("Stack Text", typeof(RectTransform));
			obj.transform.SetParent(displayParent, false);

			TextMeshProUGUI text = obj.AddComponent<TextMeshProUGUI>();
			text!.autoSizeTextContainer = true;
			text.text = stack.quantity.ToString();
			text.color = Color.white;
			text.fontSize = STACK_TEXT_SIZE;

			ContentSizeFitter sizeFitter = obj.AddComponent<ContentSizeFitter>();
			sizeFitter.verticalFit = sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

			RectTransform rect = obj.GetComponent<RectTransform>();
			rect.pivot = new Vector2(1f, 0f);
			rect.anchorMax = new Vector2(0.9375f, 0.0625f);
			rect.anchorMin = rect.anchorMax;
			rect.anchoredPosition = Vector2.zero;
			//rect.anchoredPosition = new Vector2(Mathf.Max(-4, rect.sizeDelta.x - 19.14f), rect.anchoredPosition.y);

			return text;
		}
	}
}
