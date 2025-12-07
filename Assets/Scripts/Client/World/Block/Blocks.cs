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
		private static Dictionary<int, Block> blocksByHashedId = new();
		//[BlockCache(nameof(air))]public static Block air => Lookup(() => new AirBlock());
		//[BlockCache(nameof(grass))]public static Block grass => Lookup(() => new GenericBlock("Grass Block", Tile("grass"), Items.grassBlock, new BreakRequirement(1, ToolType.None, 10)));
		//[BlockCache(nameof(dirt))]public static Block dirt => Lookup(() => new GenericBlock("Dirt Block", Tile("dirt"), Items.dirtBlock, new BreakRequirement(1, ToolType.None, 10)));
		//[BlockCache(nameof(stone))]public static Block stone => Lookup(() => new GenericBlock("Stone Block", Tile("stone"), Items.stoneBlock, new BreakRequirement(1, ToolType.None, 10)));
		//[BlockCache(nameof(wood))]public static Block wood => Lookup(() => new GenericBlock("Wood", Tile("wood"), Items.woodBlock, new BreakRequirement(0, ToolType.None, 10)));
		//[BlockCache(nameof(leaves))]public static Block leaves => Lookup(() => new LeafBlock());

		public static readonly Block air = new AirBlock();
		public static readonly Block grass = new GrassBlock();
		public static readonly Block dirt = new DirtBlock();
		public static readonly Block stone = new StoneBlock();
		public static readonly Block wood = new WoodBlock();
		//public static readonly Block grass = Register("grass", new GenericBlock("Grass Block", Tile("grass"), Items.grassBlock, new BreakRequirement(1, ToolType.None, 10)));
		//public static readonly Block dirt = Register("dirt", new GenericBlock("Dirt Block", Tile("dirt"), Items.dirtBlock, new BreakRequirement(1, ToolType.None, 10)));
		//public static readonly Block stone = Register("stone", new GenericBlock("Stone Block", Tile("stone"), Items.stoneBlock, new BreakRequirement(1, ToolType.None, 10)));
		//public static readonly Block wood = Register("wood", new GenericBlock("Wood", Tile("wood"), Items.woodBlock, new BreakRequirement(0, ToolType.None, 10)));
		public static readonly Block leaves = new LeafBlock();

		static Blocks() {
			foreach (var property in typeof(Blocks).GetProperties(BindingFlags.Static | BindingFlags.Public)) {
				var cacheAttribute = property.GetCustomAttribute<BlockCache>();
				if (cacheAttribute != null) {
					ICachedRegistry<Block>.RegisterCachedReference<BlockCache>(cacheAttribute, property);
				}
			}
		}

		private static Block Register(string id, Block block) {
			int hashed = HashHelper.StableHash(id);
			block.hashedID = hashed;
			blocksByHashedId[hashed] = block;
			return block;
		}

		private static TBlock Lookup<TBlock>(Func<TBlock> instanceSupplier, [CallerMemberName] string propertyName = null) where TBlock : Block {
			return (TBlock)ICachedRegistry<Block>.Lookup(propertyName, instanceSupplier);
		}

		private static TileBase Tile(string name) {
			return IResourceModule.Resource<TileBase, ResourceGroups.Tiles>(name);
		}

		public static Block ByHashedID(int hashedID) {
			//if (ICachedRegistry<Block>.cached.TryGetValue(hashedID, out Block block)) {
			//	return block;
			//}
			//if (ICachedRegistry<Block>.cachedReferences.TryGetValue(hashedID, out Func<Block> reference)) {
			//	return reference.Invoke();
			//}
			//throw new KeyNotFoundException($"Block hashedID {hashedID} not found.");
			return blocksByHashedId[hashedID];
		}

		public sealed class GrassBlock : Block {
			public GrassBlock() : base("grass") {
			}

			public override string name => "Grass";
			public override TileBase tileReference => Tile("grass");
			public override BlockItem itemReference => Items.grassBlock;

			public override IEnumerable<ItemStack> GetDrops(BlockState blockState, BreakSource source) {
				yield break;
			}

			protected override BlockState CreateDefaultState(BlockPropertyPool propertyPool) {
				return new(this, propertyPool.CreateEntries());
			}

			protected override void RegisterProperties(BlockPropertyPool pool) {
			}
		}

		public sealed class StoneBlock : Block {
			public StoneBlock() : base("stone") {
			}

			public override string name => "Stone";
			public override TileBase tileReference => Tile("stone");
			public override BlockItem itemReference => Items.stoneBlock;

			public override IEnumerable<ItemStack> GetDrops(BlockState blockState, BreakSource source) {
				yield break;
			}

			protected override BlockState CreateDefaultState(BlockPropertyPool propertyPool) {
				return new(this, propertyPool.CreateEntries());
			}

			protected override void RegisterProperties(BlockPropertyPool pool) {
			}
		}

		public sealed class DirtBlock : Block {
			public DirtBlock() : base("dirt") {
			}

			public override string name => "Dirt";
			public override TileBase tileReference => Tile("dirt");
			public override BlockItem itemReference => Items.dirtBlock;

			public override IEnumerable<ItemStack> GetDrops(BlockState blockState, BreakSource source) {
				yield break;
			}

			protected override BlockState CreateDefaultState(BlockPropertyPool propertyPool) {
				return new(this, propertyPool.CreateEntries());
			}

			protected override void RegisterProperties(BlockPropertyPool pool) {
			}
		}


		public sealed class WoodBlock : Block {
			public WoodBlock() : base("wood") {
			}

			public override string name => "Wood";
			public override TileBase tileReference => Tile("wood");
			public override BlockItem itemReference => Items.woodBlock;

			public override IEnumerable<ItemStack> GetDrops(BlockState blockState, BreakSource source) {
				yield break;
			}

			protected override BlockState CreateDefaultState(BlockPropertyPool propertyPool) {
				return new(this, propertyPool.CreateEntries());
			}

			protected override void RegisterProperties(BlockPropertyPool pool) {
			}
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
