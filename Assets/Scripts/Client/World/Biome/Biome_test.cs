using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Client.World.Biome {
	public class Biome_test : IBiome {
		private readonly int platformHeight;

		public Biome_test(int platformHeight) {
			this.platformHeight = platformHeight;
		}

		public IColumnGenerator CreateColumnGenerator(int colX) {
			return new SimpleHeightmapColumnGenerator(this, colX >= Level.CHUNK_LENGTH / 3 ? platformHeight + colX : platformHeight);
		}

		public BlockState ResolveBlock(float colDensity, int x, int y) {
			return colDensity < 0 ? Blocks.stone.defaultState : Blocks.air.defaultState;
		}
	}
}
