namespace SoulboundEngine.Command {
	using Brigadier.NET;
	using Brigadier.NET.ArgumentTypes;
	using Brigadier.NET.Exceptions;
	using SoulboundEngine.Common;
	using System;

	public class GuidArgumentType : ArgumentType<Guid> {
		public override Guid Parse(IStringReader reader) {
			int cursor = reader.Cursor;
			string token = reader.ReadString();

			if (!Guid.TryParse(token, out Guid guid)) {
				reader.Cursor = cursor;
				throw new DynamicCommandExceptionType(o => new LiteralMessage("Invalid guid '{}'".WithArgs(o))).CreateWithContext(reader, token);
			}
			return guid;
		}
	}
}
