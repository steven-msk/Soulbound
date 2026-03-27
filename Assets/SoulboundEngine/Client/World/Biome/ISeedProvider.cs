using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.World.Generation {
	public interface ISeedProvider {
		int GetSeed();
	}
}
