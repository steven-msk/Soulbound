using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoulboundEngine.Client.UI {
	public class InputField : IUIElementTemplate<InputFieldHandle> {
		public GameObject Instantiate() {
			GameObject obj = new("Input Field", typeof(RectTransform));

			// TMP_InputField misses the condition to create the caret object when OnEnable is called
			// the object needs to be disabled before the component is added
			obj.SetActive(false);

			Image bg = obj.AddComponent<Image>();
			bg.color = new Color(0.1f, 0.1f, 0.1f, 1f);

			GameObject textObj = new("Text", typeof(RectTransform));
			textObj.transform.SetParent(obj.transform, false);

			TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
			text.color = Color.white;
			text.fontSize = 16f;

			RectTransform textRect = textObj.GetComponent<RectTransform>();
			textRect.anchorMin = Vector2.zero;
			textRect.anchorMax = Vector2.one;
			textRect.offsetMin = new Vector2(10f, 0f);
			textRect.offsetMax = new Vector2(-10f, 0f);
			ContentSizeFitter textSizeFitter = textObj.AddComponent<ContentSizeFitter>();
			textSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

			GameObject viewport = new("Viewport", typeof(RectTransform), typeof(Mask), typeof(Image));
			viewport.transform.SetParent(obj.transform, false);

			Mask mask = viewport.GetComponent<Mask>();
			mask.showMaskGraphic = false;

			RectTransform viewportRect = viewport.GetComponent<RectTransform>();
			viewportRect.anchorMin = Vector2.zero;
			viewportRect.anchorMax = Vector2.one;
			viewportRect.offsetMin = Vector2.zero;
			viewportRect.offsetMax = Vector2.zero;

			textObj.transform.SetParent(viewportRect.transform, false);

			TMP_InputField inputField = obj.AddComponent<TMP_InputField>();
			inputField.textComponent = text;
			inputField.textViewport = viewportRect;
			inputField.lineType = TMP_InputField.LineType.SingleLine;
			inputField.onFocusSelectAll = false;
			inputField.restoreOriginalTextOnEscape = false;
			inputField.characterLimit = 100;

			LayoutElement layoutElement = obj.AddComponent<LayoutElement>();
			layoutElement.minHeight = 30f;
			layoutElement.minWidth = 300f;

			ContentSizeFitter sizeFitter = obj.AddComponent<ContentSizeFitter>();
			sizeFitter.verticalFit = sizeFitter.horizontalFit = ContentSizeFitter.FitMode.MinSize;

			// now the caret object will be created
			obj.SetActive(true);

			return obj;
		}
	}
}
