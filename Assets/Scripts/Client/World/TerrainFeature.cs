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
    public BoundsInt2D bounds;
    [CanBeNull] public Dictionary<ChunkBlockPos, BlockState> stateOverrides;

    public TerrainFeature(ChunkBlockPos origin, TerrainFeatureType type, Dictionary<ChunkBlockPos, BlockState> tileOverrides, BoundsInt2D bounds) {
        this.origin = origin;
        this.type = type;
        this.stateOverrides = tileOverrides;
        this.bounds = bounds;
    }

    public static bool operator !=(TerrainFeature feature1, TerrainFeature feature2) => !(feature1 == feature2);

    public static bool operator ==(TerrainFeature feature1, TerrainFeature feature2) {
        bool nullOrEmptyOverrides1 = feature1.stateOverrides == null || feature1.stateOverrides.Count == 0;
        bool nullOrEmptyOverrides2 = feature2.stateOverrides == null || feature2.stateOverrides.Count == 0;
        return feature1.origin == feature2.origin &&
               feature1.type == feature2.type &&
               !nullOrEmptyOverrides1 && !nullOrEmptyOverrides2 &&
               feature1.stateOverrides.Count == feature2.stateOverrides.Count &&
               feature1.stateOverrides.All(kvp => feature2.stateOverrides.TryGetValue(kvp.Key, out var state) && state == kvp.Value);
    }

    public bool PersistentExistence() {
        return this.stateOverrides != null && this.stateOverrides.Count > 0;
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
        if (stateOverrides != null) {
            foreach (var stateOverride in stateOverrides) {
                hash = hash * 31 + stateOverride.Key.GetHashCode();
                hash = hash * 31 + (stateOverride.Value != null ? stateOverride.Value.GetHashCode() : 0);
            }
        }
        return hash;
    }

    public override string ToString() {
        StringBuilder sb = new StringBuilder();
        sb.Append($"TerrainFeature @ {origin}, Type: {type}");
        sb.Append($", Bounds: {bounds}");
        return sb.ToString();
    }
}