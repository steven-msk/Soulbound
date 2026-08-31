namespace SoulboundEngine.Command {
	using Brigadier.NET;
	using Brigadier.NET.ArgumentTypes;
	using Brigadier.NET.Exceptions;

	public class CoordinateArgumentType : ArgumentType<Coordinate> {
		private readonly bool allowRelative;

		public CoordinateArgumentType(bool allowRelative = true) {
			this.allowRelative = allowRelative;
		}

		public override Coordinate Parse(IStringReader reader) {
			char prefix = reader.Peek();
			bool useTarget = prefix == '^';
			bool isRelative = this.allowRelative && prefix == '~' || useTarget;
			if (isRelative) reader.Skip();

			if (reader.RemainingLength <= 0 && this.allowRelative) {
				return new Coordinate(isRelative, 0.0d, useTarget);
			}

			bool hasValue = false;
			double value = 0.0d;
			char peek = reader.Peek();
			if (char.IsDigit(peek) || peek == '-') {
				hasValue = true;
				value = reader.ReadDouble();
			} else if (peek != ' ') {
				throw CommandSyntaxException.BuiltInExceptions.ReaderExpectedDouble().CreateWithContext(reader);
			}
			return !hasValue && !isRelative
				? throw new SimpleCommandExceptionType(new LiteralMessage("Invalid coordinate")).CreateWithContext(reader)
				: new Coordinate(isRelative, value, useTarget);
		}
	}
}
