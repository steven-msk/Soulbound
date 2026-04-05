using Brigadier.NET;
using Brigadier.NET.ArgumentTypes;
using Brigadier.NET.Exceptions;

namespace SoulboundEngine.Client.Debug.Commands {
	public class CoordinateArgumentType : ArgumentType<Coordinate> {
		public override Coordinate Parse(IStringReader reader) {
			int cursor = reader.Cursor;
			string token = reader.ReadString();

			if (token.StartsWith("~")) {
				string rem = token[1..];

				if (string.IsNullOrEmpty(rem)) {
					reader.Cursor = cursor;
					throw CommandSyntaxException.BuiltInExceptions.ReaderExpectedInt().CreateWithContext(reader);
				}
				if (!float.TryParse(rem, out float relativeOffset)) {
					reader.Cursor = cursor;
					throw CommandSyntaxException.BuiltInExceptions.ReaderInvalidFloat().CreateWithContext(reader, rem);
				}

				return new Coordinate {
					isRelative = true,
					value = relativeOffset
				};
			}

			if (!float.TryParse(token, out float absoluteValue)) {
				reader.Cursor = cursor;
				throw CommandSyntaxException.BuiltInExceptions.ReaderInvalidFloat().CreateWithContext(reader, token);
			}
			return new Coordinate {
				isRelative = false,
				value = absoluteValue
			};
		}
	}
}
