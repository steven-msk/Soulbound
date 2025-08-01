using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct TerrainFeature {
    public ChunkBlockPos origin;
    public TerrainFeatureType type;
    [CanBeNull] public Dictionary<ChunkBlockPos, TileBase> tileOverrides;

    public TerrainFeature(ChunkBlockPos origin, TerrainFeatureType type, Dictionary<ChunkBlockPos, TileBase> tileOverrides) {
        this.origin = origin;
        this.type = type;
        this.tileOverrides = tileOverrides;
    }

    public static bool operator !=(TerrainFeature feature1, TerrainFeature feature2) => !(feature1 == feature2);

    public static bool operator ==(TerrainFeature feature1, TerrainFeature feature2) {
        bool nullOrEmptyOverrides1 = feature1.tileOverrides == null || feature1.tileOverrides.Count == 0;
        bool nullOrEmptyOverrides2 = feature2.tileOverrides == null || feature2.tileOverrides.Count == 0;
        return feature1.origin == feature2.origin &&
               feature1.type == feature2.type &&
               !nullOrEmptyOverrides1 && !nullOrEmptyOverrides2 &&
               feature1.tileOverrides.Count == feature2.tileOverrides.Count &&
               feature1.tileOverrides.All(kvp => feature2.tileOverrides.TryGetValue(kvp.Key, out var tile) && tile == kvp.Value);
    }

    public bool UncheckedExistence() {
        return this.tileOverrides != null && this.tileOverrides.Count > 0;
    }

    public override bool Equals(object obj) {
        if (obj is TerrainFeature other) {
            return this == other;
        }
        return false;
    }

    public override int GetHashCode() {
        int hash = 17;
        hash = hash * 31 + origin.GetHashCode();
        hash = hash * 31 + type.GetHashCode();
        if (tileOverrides != null) {
            foreach (var tileOverride in tileOverrides) {
                hash = hash * 31 + tileOverride.Key.GetHashCode();
                hash = hash * 31 + (tileOverride.Value != null ? tileOverride.Value.GetHashCode() : 0);
            }
        }
        return hash;
    }

    public override string ToString() {
        StringBuilder sb = new StringBuilder();
        sb.Append($"TerrainFeature @ {origin}, Type: {type}");
        if (tileOverrides != null && tileOverrides.Count > 0) {
            sb.Append(", Tile Overrides: ");
            foreach (var tileOverride in tileOverrides) {
                sb.Append($"[{tileOverride.Key} -> {tileOverride.Value?.name ?? "null"}] ");
            }
        }
        return sb.ToString();
    }
}