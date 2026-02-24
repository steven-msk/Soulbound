using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core.Debug.Commands {
	public sealed class CoordinateParser : ICommandArgumentParser<Coordinate> {
		public bool TryParse(string token, out Coordinate value) {
			value = default;

			if (token.StartsWith("~")) {
				string remainder = token[1..];
				float offset = 0;

				if (!string.IsNullOrEmpty(remainder)) {
					if (!float.TryParse(remainder, out offset)) {
						return false;
					}
				}

				value = new Coordinate {
					isRelative = true,
					value = offset
				};
			}

			if (float.TryParse(token, out float absolute)) {
				value = new Coordinate {
					isRelative = false,
					value = absolute
				};
				return true;
			}

			return false;
		}
	}
}
