using SoulboundBackend.Client.World.BlockSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.Generation {
	public interface IBiome {
		IColumnGenerator CreateColumnGenerator(int colX);

		BlockState ResolveBlock(float colDensity, int x, int y);
	}
}
