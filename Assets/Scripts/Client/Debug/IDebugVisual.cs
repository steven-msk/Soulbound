using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;

public interface IDebugVisual<TValue> {
    public TextMeshProUGUI TextComponent { get; }

    public string FormatValue(TValue value);

    public virtual TextMeshProUGUI UpdateDisplayComponent(TValue value) {
        TextComponent.text = FormatValue(value);
        return TextComponent;
    }
}
