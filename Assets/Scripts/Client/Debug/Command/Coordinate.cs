using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.Debug.Commands {
	public struct Coordinate {
		public bool isRelative;
		public float value;

		public readonly float GetPos(float relative) {
			return isRelative ? relative + value : value;
		}
	}
}
