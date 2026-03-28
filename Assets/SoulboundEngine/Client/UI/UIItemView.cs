using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoulboundEngine.Client.ItemSystem.Render {
	public sealed class UIItemView : MonoBehaviour {
		private TextMeshProUGUI stackText;
		private Image itemImage;

		public void Init(TextMeshProUGUI stackText, Image itemImage) {
			this.stackText = stackText;
			this.itemImage = itemImage;
		}

		public TextMeshProUGUI GetStackText() => stackText;
		public Image GetItemImage() => itemImage;

		public void Destroy() => Destroy(gameObject);
	}
}
