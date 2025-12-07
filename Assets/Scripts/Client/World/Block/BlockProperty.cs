using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.BlockSystem {
	public class BlockProperty<T> : IBlockStateProperty {
		public string name { get; }
		Type IBlockStateProperty.valueType => typeof(T);

		internal BlockProperty(string name) {
			this.name = name;
		}

		public override string ToString() => name;
	}
}
