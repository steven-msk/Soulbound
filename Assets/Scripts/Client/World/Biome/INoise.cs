using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.Generation {
	public interface INoise {
		/// <returns>
		/// Noise output bounded between -1...1
		/// </returns>
		float Sample1D(float x);

		/// <returns>
		/// Noise output bounded between -1...1
		/// </returns>
		float Sample2D(float x, float y);

		/// <returns>
		/// Noise output bounded between -1...1
		/// </returns>
		float Sample3D(float x, float y, float z);
	}
}
