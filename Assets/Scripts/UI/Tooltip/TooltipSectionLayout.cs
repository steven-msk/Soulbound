using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipSectionLayout {
	public TooltipSection Section { get; set; } = TooltipSection.Info;
	public int fontSize = 10;
	public Color textColor = Color.white;
	public FontStyles fontStyle = FontStyles.Normal;
	public TextAlignmentOptions alignment = TextAlignmentOptions.Left;
	public Vector4 margin = Vector4.zero;
	public TMP_FontAsset fontAsset = Registry.Get<TMP_FontAsset>("Urbanist-SemiBold SDF");
	//...animations, shaders?

	public TooltipSectionLayout(TooltipSection section, bool applyPreset = true) {
		Section = section;
		if (applyPreset) {
			section.ApplyLayoutPreset(this);
		}
	}

	public void Apply(TextMeshProUGUI textComponent) {
		textComponent.font = fontAsset;
		textComponent.color = textColor;
		textComponent.fontSize = fontSize;
		textComponent.fontStyle = fontStyle;
		textComponent.alignment = alignment;
		textComponent.margin = margin;
	}
}