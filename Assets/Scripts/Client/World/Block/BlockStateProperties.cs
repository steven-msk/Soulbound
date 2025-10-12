using NUnit.Framework.Constraints;
using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.BlockSystem {
	public class BlockStateProperties : ReadOnlyDictionary<string, object> {
		public BlockStateProperties(IDictionary<string, object> predefined) 
			: base(predefined) {
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
