using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Plastic.Newtonsoft.Json;

public class StructurePlacement {
    public ChunkBlockPos origin { get; private set; }
    public string ID { get; private set; }
    public BoundsInt2D bounds { get; private set; }

    [JsonConverter(typeof(JsonDictionaryConverter<ChunkBlockPos, BlockState>))]
	public Dictionary<ChunkBlockPos, BlockState> stateOverrides { get; set; } = new();

    public StructurePlacement(ChunkBlockPos origin, string ID, Dictionary<ChunkBlockPos, BlockState> stateOverrides, BoundsInt2D bounds) {
        this.origin = origin;
        this.ID = ID;
        this.stateOverrides = stateOverrides;
        this.bounds = bounds;
    }

    public static bool operator !=(StructurePlacement structure1, StructurePlacement structure2) => !(structure1 == structure2);

    public static bool operator ==(StructurePlacement structure1, StructurePlacement structure2) {
        return structure1.origin == structure2.origin &&
               structure1.ID.Equals(structure2.ID) &&
               structure1.stateOverrides.Count == structure2.stateOverrides.Count &&
               structure1.stateOverrides.All(kvp => structure2.stateOverrides.TryGetValue(kvp.Key, out var state) && state == kvp.Value);
    }

    public bool PersistentExistence() {
        return this.stateOverrides != null && this.stateOverrides.Count > 0;
    }

    public override bool Equals(object obj) {
        if (obj is StructurePlacement other) {
            return this == other;
        }
        return false;
    }

    public override int GetHashCode() {
        int hash = 17;
        hash = hash * 31 + origin.GetHashCode();
        hash = hash * 31 + ID.GetHashCode();
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
        sb.Append($"StructurePlacement[{ID}] @ {origin}");
        sb.Append($", Bounds: {bounds}");
        return sb.ToString();
    }
}