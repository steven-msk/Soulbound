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
	public partial class Blocks : IResourceModule, ICachedRegistry<Block> {
		[BlockCache(nameof(air))] public static Block air => Lookup(() => new GenericBlock("Air", Tile("air"), null, StateCaching.Static(), null));
		[BlockCache(nameof(grass))] public static Block grass => Lookup(() => new GenericBlock("Grass Block", Tile("grass"), Items.grassBlock, StateCaching.Static(), new BreakRequirement(1, ToolType.None, 10)));
		[BlockCache(nameof(dirt))] public static Block dirt => Lookup(() => new GenericBlock("Dirt Block", Tile("dirt"), Items.dirtBlock, StateCaching.Static(), new BreakRequirement(1, ToolType.None, 10)));
		[BlockCache(nameof(stone))] public static Block stone => Lookup(() => new GenericBlock("Stone Block", Tile("stone"), Items.stoneBlock, StateCaching.Static(), new BreakRequirement(1, ToolType.None, 10)));
		[BlockCache(nameof(wood))] public static Block wood => Lookup(() => new GenericBlock("Wood", Tile("wood"), Items.woodBlock, StateCaching.Static(), new BreakRequirement(0, ToolType.None, 10)));
		[BlockCache(nameof(leaves))] public static Block leaves => Lookup(() => 
			new GenericBlock(
				"Leaves", 
				Tile("leaves"), 
				Items.leavesBlock, 
				StateCaching.Predefined(), 
				new BreakRequirement(0, ToolType.All, 10),
				defaultState: block => {
					Func<BlockState, BreakSource, bool> dropPredicate = (blockState, breakSource) => 
						breakSource is PlayerToolBreakSource || blockState.Get<bool>("persistent");
					return new BlockState(block, null, CommonBlockBehaviors.DropSingleIf(dropPredicate));
				}, 
				propertyRegisterer: instance => {
					instance.RegisterProperty(new BlockProperty<bool>("persistent"), false);
				}, 
				placeFunction: (block, itemStack, blockPos) => {
					return block.defaultState.With("persistent", true);
				}
		));
	
		static Blocks() {
			foreach (var property in typeof(Blocks).GetProperties(BindingFlags.Static | BindingFlags.Public)) {
				var cacheAttribute = property.GetCustomAttribute<BlockCache>();
				if (cacheAttribute != null) {
					ICachedRegistry<Block>.RegisterCachedReference<BlockCache>(cacheAttribute, property);
				}
			}
		}

		private static TBlock Lookup<TBlock>(Func<TBlock> instanceSupplier, [CallerMemberName] string propertyName = null) where TBlock : Block {
			return (TBlock)ICachedRegistry<Block>.Lookup(propertyName, instanceSupplier);
		}

		private static TileBase Tile(string name) {
			return IResourceModule.Resource<TileBase, ResourceGroups.Tiles>(name);
		}

		public static Block ByHashedID(int hashedID) {
			if (ICachedRegistry<Block>.cached.TryGetValue(hashedID, out Block block)) {
				return block;
			}
			if (ICachedRegistry<Block>.cachedReferences.TryGetValue(hashedID, out Func<Block> reference)) {
				return reference.Invoke();
			}
			throw new KeyNotFoundException($"Block hashedID {hashedID} not found.");
		}
	}

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class BlockCache : Attribute, ICachedReferenceAttribute {
		public string propertyName { get; set; }

		public BlockCache(string propertyName) {
			this.propertyName = propertyName;
		}
	}

    [JsonConverter(typeof(Block.BlockJsonConverter))]
    abstract partial class Block : IHashableReference {
        public int hashedID { get; set; }

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
