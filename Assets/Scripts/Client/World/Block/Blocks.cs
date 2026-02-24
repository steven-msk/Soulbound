using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Common;
using SoulboundBackend.Core.AssetManagement;
using SoulboundBackend.Core.Resource;
using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;

namespace SoulboundBackend.Client.World.BlockSystem {
	public partial class Blocks : IResourceModule, ICachedRegistry<Block> {
		private static Dictionary<int, Block> blocksByHashedId = new();

		public static readonly Block air = new GenericBlock("air", "Air", null, null, null);
		public static readonly Block grass = new GenericBlock("grass", "Grass Block", new AssetKey("grass"), Items.grassBlock, new BreakRequirement(1, ToolType.None, 10));
		public static readonly Block dirt = new GenericBlock("dirt", "Dirt Block", new AssetKey("dirt"), Items.dirtBlock, new BreakRequirement(1, ToolType.None, 10));
		public static readonly Block stone = new GenericBlock("stone", "Stone Block", new AssetKey("stone"), Items.stoneBlock, new BreakRequirement(1, ToolType.None, 10));
		public static readonly Block wood = new GenericBlock("wood", "Wood", new AssetKey("wood"), Items.woodBlock, new BreakRequirement(0, ToolType.None, 10));
		public static readonly Block leaves = new LeafBlock();

		[Obsolete]
		static Blocks() {
			foreach (var property in typeof(Blocks).GetProperties(BindingFlags.Static | BindingFlags.Public)) {
				var cacheAttribute = property.GetCustomAttribute<BlockCache>();
				if (cacheAttribute != null) {
					ICachedRegistry<Block>.RegisterCachedReference<BlockCache>(cacheAttribute, property);
				}
			}
		}

		[Obsolete]
		public static Block ByHashedID(int hashedID) {
			return blocksByHashedId[hashedID];
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
