using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.Generation {
	public interface INoise {
		float Sample(float arg);
		float Sample2D(float x, float y);
		float Sample3D(float x, float y, float z);
	}
}
