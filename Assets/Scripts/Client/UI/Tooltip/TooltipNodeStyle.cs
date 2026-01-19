using SoulboundBackend.Core.AssetManagement;
using SoulboundBackend.Core.Resource;
using TMPro;
using UnityEngine;

namespace SoulboundBackend.Client.UI.Tooltip {
	public class TooltipNodeStyle {
		public int fontSize = 10;
		public Color textColor = Color.white;
		public FontStyles fontStyle = FontStyles.Normal;
		public TextAlignmentOptions alignment = TextAlignmentOptions.Left;
		public Vector4 margin = Vector4.zero;
		public TMP_FontAsset fontAsset = ResourceManager.Get<TMP_FontAsset, ResourceGroups.Fonts>(new AssetKey("Urbanist-SemiBold SDF"));
		//...animations, shaders?

		public void Apply(TextMeshProUGUI textComponent) {
			textComponent.font = fontAsset;
			textComponent.color = textColor;
			textComponent.fontSize = fontSize;
			textComponent.fontStyle = fontStyle;
			textComponent.alignment = alignment;
			textComponent.margin = margin;
		}
	}
}