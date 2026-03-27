using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.Debug.Commands {
	public sealed class CoordinateParser : ICommandArgumentParser<Coordinate> {
		public ParseResult<Coordinate> TryParse(string token, CommandParsingContext ctx) {
			if (token.StartsWith("~")) {
				string remainder = token[1..];
				float offset = 0;

				if (!string.IsNullOrEmpty(remainder)) {
					if (!float.TryParse(remainder, out offset)) {
						return ParseResult<Coordinate>.Fail();
					}
				}
				
				Coordinate coord = new() {
					isRelative = true,
					value = offset
				};
				return ParseResult<Coordinate>.Success(coord);
			}

			if (float.TryParse(token, out float absolute)) {
				Coordinate coord = new() {
					isRelative = false,
					value = absolute
				};
				return ParseResult<Coordinate>.Success(coord);
			}

			return ParseResult<Coordinate>.Fail();
		}
	}
}
