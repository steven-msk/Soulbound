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
		private SpriteAnimationPlayer animationPlayer;

		public void Init(TextMeshProUGUI stackText, Image itemImage, SpriteAnimationPlayer animationPlayer) {
			this.stackText = stackText;
			this.itemImage = itemImage;
			this.animationPlayer = animationPlayer;
			rect = GetComponent<RectTransform>();
		}

		public TextMeshProUGUI GetStackText() => stackText;
		public Image GetItemImage() => itemImage;
		public SpriteAnimationPlayer GetAnimationPlayer() => animationPlayer;

		public void Destroy() => Destroy(gameObject);

		public void SetPosition(Vector2 position) {
			rect.transform.position = position;
		}

		public void SetParent(RectTransform rectParent) {
			rect.SetParent(rectParent, false);
		}

		Sprite IAnimationTarget<Sprite>.Get() => itemImage.sprite;
		void IAnimationTarget<Sprite>.Set(Sprite value) => itemImage.sprite = value;
	}
}
