using SoulboundBackend.Client.SettingSystem;
using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SoulboundBackend.Client.SettingSystem.View {
	[PROTOTYPICAL]
	public static class SliderFactory {
		public static Slider CreateSlider(Transform parent) {
			GameObject sliderObject = new("Slider", typeof(RectTransform), typeof(Slider));
			sliderObject.transform.SetParent(parent, false);

			RectTransform rootRect = sliderObject.GetComponent<RectTransform>();
			rootRect.sizeDelta = new Vector2(80f, 10f);

			GameObject background = new("Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
			background.transform.SetParent(sliderObject.transform, false);
			RectTransform bgRect = background.GetComponent<RectTransform>();
			SetStretchedCustomAnchor(bgRect);
			bgRect.offsetMin = new Vector2(0f, 0f);
			bgRect.offsetMax = new Vector2(0f, 0f);
			Image bgImage = background.GetComponent<Image>();
			bgImage.color = new Color(1f, 1f, 1f, 0.25f);

			GameObject fillArea = new("Fill Area", typeof(RectTransform));
			fillArea.transform.SetParent(sliderObject.transform, false);
			RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
			SetStretchedCustomAnchor(fillAreaRect);
			fillAreaRect.offsetMin = new Vector2(5f, 0f);
			fillAreaRect.offsetMax = new Vector2(-15f, 0f);

			GameObject fill = new("Fill", typeof(RectTransform), typeof(Image));
			fill.transform.SetParent(fillArea.transform, false);
			RectTransform fillRect = fill.GetComponent<RectTransform>();
			fillRect.anchorMin = new Vector2(0f, 0f);
			fillRect.anchorMax = new Vector2(0f, 1f);
			fillRect.sizeDelta = new Vector2(10f, 0);
			Image fillImage = fill.GetComponent<Image>();
			fillImage.color = new Color(0.3f, 0.6f, 1f, 1f);

			GameObject handleArea = new("Handle Slide Area", typeof(RectTransform));
			handleArea.transform.SetParent(sliderObject.transform, false);
			RectTransform handleAreaRect = handleArea.GetComponent<RectTransform>();
			handleAreaRect.anchorMin = new Vector2(0f, 0f);
			handleAreaRect.anchorMax = new Vector2(1f, 1f);
			handleAreaRect.offsetMin = new Vector2(5f, 0f);
			handleAreaRect.offsetMax = new Vector2(-5f, 0f);

			GameObject handle = new("Handle", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
			handle.transform.SetParent(handleArea.transform, false);
			RectTransform handleRect = handle.GetComponent<RectTransform>();
			SetStretchedLeftAnchor(handleRect);
			handleRect.sizeDelta = new Vector2(10f, 10f);
			Image handleImage = handle.GetComponent<Image>();
			handleImage.color = Color.white;

			Slider slider = sliderObject.GetComponent<Slider>();
			slider.fillRect = fillRect;
			slider.handleRect = handleRect;
			slider.targetGraphic = handleImage;
			slider.direction = Slider.Direction.LeftToRight;
			slider.minValue = 0f;
			slider.maxValue = 1f;

			return slider;
		}

		private static void SetStretchedCustomAnchor(RectTransform rectTransform) {
			rectTransform.anchorMin = new Vector2(0f, 0.25f);
			rectTransform.anchorMax = new Vector2(1f, 0.75f);
		}

		private static void SetStretchedLeftAnchor(RectTransform rectTransform) {
			rectTransform.anchorMin = new Vector2(0f, 0f);
			rectTransform.anchorMax = new Vector2(0f, 1f);
		}
	}
}
