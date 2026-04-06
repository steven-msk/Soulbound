using Brigadier.NET;
using Brigadier.NET.ArgumentTypes;
using Brigadier.NET.Exceptions;

namespace SoulboundEngine.Client.Debug.Commands {
	public class CoordinateArgumentType : ArgumentType<Coordinate> {
		public override Coordinate Parse(IStringReader reader) {
			bool isRelative = reader.Peek() == '~';
			if (isRelative) reader.Skip();

			float value = 0f;
			char peek = reader.Peek();

			if (char.IsDigit(peek) || peek == '-') {
				value = reader.ReadFloat();
			} else if (peek != ' ') {
				throw CommandSyntaxException.BuiltInExceptions.ReaderExpectedFloat().CreateWithContext(reader);
			}

			return new Coordinate {
				isRelative = isRelative,
				value = value
			};
		}
	}
}
