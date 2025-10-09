using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Common;
using SoulboundBackend.Core.Resource;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine.Tilemaps;

namespace SoulboundBackend.Client.World.BlockSystem {
    public partial class Blocks : IResourceModule {
        // REMINDER: since block state properties are unavailable as of right now, keep in mind that block behavior definitions might change when they have actual purpose
        private static ConcurrentDictionary<int, Block> cached = new();

        public static Block air => Lookup("air", () => new GenericBlock("Air", Tile("air"), null));
        public static Block grass => Lookup("grass", () => new GenericBlock("Grass Block", Tile("grass"), Items.grassBlock));
        public static Block dirt => Lookup("dirt", () => new GenericBlock("Dirt Block", Tile("dirt"), Items.dirtBlock));
        public static Block stone => Lookup("stone", () => new GenericBlock("Stone Block", Tile("stone"), Items.stoneBlock));
        public static Block wood => Lookup("wood", () => new GenericBlock("Wood", Tile("wood"), Items.woodBlock));
        public static Block leaves => Lookup("leaves", () => new GenericBlock("Leaves", Tile("leaves"), Items.leavesBlock, _ => BlockBehaviors.DropIfPlayerBroke()));

        private static TileBase Tile(string name) {
            return IResourceModule.Resource<TileBase, ResourceGroups.Tiles>(name);
        }

        private static TBlock Lookup<TBlock>(string key, Func<TBlock> instanceSupplier) where TBlock : Block {
            int hash = HashHelper.StableHash(key);

            return (TBlock)cached.GetOrAdd(hash, hashedID => {
                TBlock block = instanceSupplier.Invoke();
                block.hashedID = hashedID;
                return block;
            });
        }

        public static Block ByHashedID(int hashedID) {
            if (cached.TryGetValue(hashedID, out Block block)) {
                return block;
            }
            throw new KeyNotFoundException($"Block hashedID {hashedID} not found."); 
        }
    }

    [JsonConverter(typeof(Block.BlockJsonConverter))]
    abstract partial class Block {
        public int hashedID { get; internal set; }

	    public sealed class BlockJsonConverter : JsonConverter<Block> {
		    public override Block ReadJson(JsonReader reader, Type objectType, Block existingValue, bool hasExistingValue, JsonSerializer serializer) {
                int id = Convert.ToInt32(reader.Value);
			    return Blocks.ByHashedID(id);
		    }

		    public override void WriteJson(JsonWriter writer, Block value, JsonSerializer serializer) {
			    writer.WriteValue(value.hashedID);
		    }
	    }
    }
}
