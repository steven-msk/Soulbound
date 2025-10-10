using Codice.CM.Client.Differences;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Common;
using SoulboundBackend.Core.Resource;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine.Tilemaps;

namespace SoulboundBackend.Client.World.BlockSystem {
    public partial class Blocks : IResourceModule {
        // REMINDER: since block state properties are unavailable as of right now, keep in mind that block behavior definitions might change when they have actual purpose
        private static ConcurrentDictionary<int, Block> cached = new();
        private static ConcurrentDictionary<int, Func<Block>> cachedReferences = new();

        [BlockCache(nameof(air))] public static Block air => Lookup(() => new GenericBlock("Air", Tile("air"), null));
        [BlockCache(nameof(grass))] public static Block grass => Lookup(() => new GenericBlock("Grass Block", Tile("grass"), Items.grassBlock));
        [BlockCache(nameof(dirt))] public static Block dirt => Lookup(() => new GenericBlock("Dirt Block", Tile("dirt"), Items.dirtBlock));
        [BlockCache(nameof(stone))] public static Block stone => Lookup(() => new GenericBlock("Stone Block", Tile("stone"), Items.stoneBlock));
        [BlockCache(nameof(wood))] public static Block wood => Lookup(() => new GenericBlock("Wood", Tile("wood"), Items.woodBlock));
        [BlockCache(nameof(leaves))] public static Block leaves => Lookup(() => new GenericBlock("Leaves", Tile("leaves"), Items.leavesBlock, null, _ => CommonBlockBehaviors.DropIfPlayerBroke()));
    
        static Blocks() {
            foreach (var property in typeof(Blocks).GetProperties(BindingFlags.Static | BindingFlags.Public)) {
                var cacheAttribute = property.GetCustomAttribute<BlockCache>();
                if (cacheAttribute != null) {
                    RegisterBlockCache(cacheAttribute, property);
                }
            }
        }

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

        private static TBlock Lookup<TBlock>(Func<TBlock> instanceSupplier, [CallerMemberName] string propertyName = null) where TBlock : Block {
            return Lookup<TBlock>(propertyName, instanceSupplier);
        }

        public static Block ByHashedID(int hashedID) {
            if (cached.TryGetValue(hashedID, out Block block)) {
                return block;
            }
            if (cachedReferences.TryGetValue(hashedID, out Func<Block> reference)) {
                return reference.Invoke();
            }
            throw new KeyNotFoundException($"Block hashedID {hashedID} not found.");
        }

        private static void RegisterBlockCache(BlockCache blockCache, PropertyInfo property) {
            var getter = property.GetGetMethod();
            if (getter == null) {
                throw new NotSupportedException("No getter found for block property: " + property);
            }

            Func<Block> accessor = () => (Block)getter.Invoke(null, null);
            int hash = HashHelper.StableHash(blockCache.PropertyName);
            cachedReferences[hash] = accessor;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class BlockCache : Attribute {
        public string PropertyName { get; set; }

        public BlockCache(string propertyName) {
            this.PropertyName = propertyName;
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
