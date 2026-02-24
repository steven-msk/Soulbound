using SoulboundBackend.Common;
using SoulboundBackend.Common.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using Unity.VisualScripting;

namespace SoulboundBackend.Client.World.BlockSystem {
	[Obsolete]
	[JsonConverter(typeof(JsonDictionaryConverter<IBlockStateProperty, object>))]
	public class BlockStateProperties : ReadOnlyDictionary<IBlockStateProperty, object> {
		public BlockStateProperties() : this(new Dictionary<IBlockStateProperty, object>()) {
		}

		public BlockStateProperties(IDictionary<IBlockStateProperty, object> predefined) 
			: base(predefined) {
		}

		public Dictionary<IBlockStateProperty, object> CloneMappings() {
			Dictionary<IBlockStateProperty, object> cloned = new();
			cloned.AddRange(this.ToList());
			return cloned;
		}

		public bool Contains(IBlockStateProperty property) {
			return this.ContainsKey(property);
		}

		public override int GetHashCode() {
			return HashHelper.StableHash(this.ToString());
		}

		public override string ToString() {
			string[] entries = this.Select(kvp => {
				return $"{kvp.Key}={kvp.Value}";
			}).ToArray();
			return "{" + string.Join(',', entries) + "}";
		}

		public override bool Equals(object obj) {
			return obj is BlockStateProperties other
				&& other.SequenceEqual(this);
		}
	}
}
