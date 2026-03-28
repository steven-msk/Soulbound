using SoulboundEngine.Core.Assets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoulboundEngine.Client.ItemSystem.Render {
	public sealed class UIItemRenderer {
		private const float IMAGE_SIZE = 32f;
		private const float STACK_TEXT_SIZE = 8f;

		public UIItemView CreateView(RectTransform parent) {
			GameObject obj = new("UI Item View", typeof(RectTransform));
			obj.transform.SetParent(parent, false);

			RectTransform rect = obj.GetComponent<RectTransform>();
			rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
			rect.sizeDelta = new Vector2(IMAGE_SIZE, IMAGE_SIZE);
			rect.anchoredPosition = Vector2.zero;

			Image itemImage = obj.AddComponent<Image>();
			itemImage.raycastTarget = false;
			TextMeshProUGUI stackText = CreateStackText(obj.GetComponent<RectTransform>());

			UIItemView view = obj.AddComponent<UIItemView>();
			view.Init(stackText, itemImage);

			obj.SetActive(false);

			return view;
		}

		private TextMeshProUGUI CreateStackText(RectTransform viewParent) {
			GameObject obj = new("Stack Text", typeof(RectTransform));
			obj.transform.SetParent(viewParent, false);

			TextMeshProUGUI text = obj.AddComponent<TextMeshProUGUI>();
			text!.autoSizeTextContainer = true;
			text.color = Color.white;
			text.fontSize = STACK_TEXT_SIZE;

			ContentSizeFitter sizeFitter = obj.AddComponent<ContentSizeFitter>();
			sizeFitter.verticalFit = sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

			RectTransform rect = obj.GetComponent<RectTransform>();
			rect.pivot = new Vector2(1f, 0f);
			rect.anchorMin = rect.anchorMax = new Vector2(0.9375f, 0.0625f);
			rect.anchoredPosition = Vector2.zero;

			return text;
		}

		public void Render(UIItemView view, ItemRenderModel model) {
			Sprite itemSprite = AssetManager.Resolve<Sprite>(model.spriteKey);
			view.GetItemImage().sprite = itemSprite;

			TextMeshProUGUI stackText = view.GetStackText();
			stackText.text = model.stackQuantity.ToString();
			stackText.enabled = model.showStackText;

			view.gameObject.SetActive(true);
		}
	}
}
