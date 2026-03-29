using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoulboundEngine.Client.ItemSystem.Render {
	[RequireComponent(typeof(RectTransform))]
	public sealed class UIItemView : MonoBehaviour {
		private RectTransform rect;
		private TextMeshProUGUI stackText;
		private Image itemImage;

		public void Init(TextMeshProUGUI stackText, Image itemImage) {
			this.stackText = stackText;
			this.itemImage = itemImage;
			rect = GetComponent<RectTransform>();
		}

		public TextMeshProUGUI GetStackText() => stackText;
		public Image GetItemImage() => itemImage;

		public void Destroy() => Destroy(gameObject);

		public void SetPosition(Vector2 position) {
			rect.transform.position = position;
		}

		public void SetParent(RectTransform rectParent) {
			rect.SetParent(rectParent, false);
		}
	}
}
