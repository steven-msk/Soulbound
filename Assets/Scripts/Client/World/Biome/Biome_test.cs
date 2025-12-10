using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Client.World.Biome {
	public class Biome_test : IBiome {
		public IColumnGenerator CreateColumnGenerator(int colX) {
			return new SimpleHeightmapColumnGenerator(this, 100);
		}

		public BlockState ResolveBlock(float colDensity, int x, int y) {
			return colDensity < 0 ? Blocks.stone.defaultState : Blocks.air.defaultState;
		}
	}
}
