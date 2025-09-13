using UnityEngine;

public class VectorCoordinateVisual : DebugVisualUpdater, IDebugVisual<Vector2> {
    [SerializeField] private string format;

    public string FormatValue(Vector2 value) => string.Format(format, value.x, value.y);
}