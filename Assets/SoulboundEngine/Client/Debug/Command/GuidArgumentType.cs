using Brigadier.NET;
using Brigadier.NET.ArgumentTypes;
using Brigadier.NET.Exceptions;
using System;

namespace SoulboundEngine.Client.Debug.Commands {
	public class GuidArgumentType : ArgumentType<Guid> {
		public override Guid Parse(IStringReader reader) {
			int cursor = reader.Cursor;
			string token = reader.ReadString();

			if (!Guid.TryParse(token, out Guid guid)) {
				reader.Cursor = cursor;
				throw CommandSyntaxException.BuiltInExceptions.ReaderExpectedSymbol().CreateWithContext(reader, token);
			}
			return guid;
		}
	}
}
