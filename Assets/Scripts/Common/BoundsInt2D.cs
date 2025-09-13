using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

[JsonConverter(typeof(BoundsInt2DJsonConverter))]
public struct BoundsInt2D {
    public int xMin;
    public int yMin;
    public int xMax;
    public int yMax;

    [JsonIgnore] public int width => xMax - xMin;
    [JsonIgnore] public int height => yMax - yMin;
    [JsonIgnore] public Vector2Int min => new Vector2Int(xMin, yMin);
    [JsonIgnore] public Vector2Int max => new Vector2Int(xMax, yMax);
    [JsonIgnore] public Vector2Int center => new Vector2Int((xMin + xMax) / 2, (yMin + yMax) / 2);
    [JsonIgnore] public Vector2Int size => new Vector2Int(width, height);

    public BoundsInt2D(Vector2Int min, Vector2Int max) {
        this.xMin = min.x;
        this.yMin = min.y;
        this.xMax = max.x;
        this.yMax = max.y;
    }

    public BoundsInt2D(int x, int y, int width, int height) {
        this.xMin = x;
        this.yMin = y;
        this.xMax = x + width;
        this.yMax = y + height;
    }

    public static explicit operator BoundsInt(BoundsInt2D bounds) {
        return new BoundsInt(new Vector3Int(bounds.xMin, bounds.yMin, 0), new Vector3Int(bounds.width, bounds.height, 1));
    }

    public static explicit operator BoundsInt2D(BoundsInt bounds) {
        return new BoundsInt2D(bounds.min.x, bounds.min.y, bounds.size.x, bounds.size.y);
    }

    public bool Contains(Vector2Int pos) {
        return pos.x >= xMin && pos.x < xMax && pos.y >= yMin && pos.y < yMax;
    }

    public override string ToString() {
        return $"BoundsInt2D(xMin: {xMin}, yMin: {yMin}, xMax: {xMax}, yMax: {yMax})";
    }
}
