using SoulboundEngine.Core.Render.Animation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoulboundEngine.Client.ItemSystem.Render {
	[RequireComponent(typeof(RectTransform))]
	public sealed class UIItemView : MonoBehaviour, IAnimationTarget<Sprite> {
		private RectTransform rect;
		private TextMeshProUGUI stackText;
		private Image itemImage;

		public void Init(TextMeshProUGUI stackText, Image itemImage) {
			this.stackText = stackText;
			this.itemImage = itemImage;
			this.rect = this.GetComponent<RectTransform>();
		}

		public TextMeshProUGUI GetStackText() => this.stackText;
		public Image GetItemImage() => this.itemImage;

		public void Destroy() => Destroy(this.gameObject);

		public void SetPosition(Vector2 position) {
			this.rect.transform.position = position;
		}

		public void SetParent(RectTransform rectParent) {
			this.rect.SetParent(rectParent, false);
		}

		Sprite IAnimationTarget<Sprite>.Get() => this.itemImage.sprite;
		void IAnimationTarget<Sprite>.Set(Sprite value) => this.itemImage.sprite = value;
	}
}
