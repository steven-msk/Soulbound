using TMPro;

public interface IDebugVisual<TValue> {
    public TextMeshProUGUI TextComponent { get; }

    public string FormatValue(TValue value);

    public virtual TextMeshProUGUI UpdateDisplayComponent(TValue value) {
        TextComponent.text = FormatValue(value);
        return TextComponent;
    }
}
