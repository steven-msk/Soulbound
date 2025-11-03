using TMPro;
using UnityEngine;

namespace SoulboundBackend.Client.Debug {

    [RequireComponent(typeof(TextMeshProUGUI))]
    public abstract class DebugVisualUpdater : MonoBehaviour {
        protected TextMeshProUGUI textComponent;
        public TextMeshProUGUI TextComponent => textComponent;

        public void Start() => textComponent = GetComponent<TextMeshProUGUI>();

        public virtual TextMeshProUGUI UpdateDisplayComponent<TValue>(IDebugVisual<TValue> debugVisual, TValue value) {
            return debugVisual.UpdateDisplayComponent(value);
        }
    }
}
