namespace SoulboundEngine.UnityClient.Debug.Command {
	using Brigadier.NET;
	using Brigadier.NET.ArgumentTypes;
	using Brigadier.NET.Context;
	using Brigadier.NET.Exceptions;
	using Brigadier.NET.Suggestion;
	using SoulboundEngine.Registry;
	using SoulboundEngine.World.Block;
	using System.Threading.Tasks;

	public class BlockArgumentType : ArgumentType<Block> {
		public override Task<Suggestions> ListSuggestions<TSource>(CommandContext<TSource> context, SuggestionsBuilder builder) {
			string remaining = builder.RemainingLowerCase;

			foreach (Block block in Registries.BLOCKS) {
				Identifier id = Blocks.GetIdentifier(block);

				if (id.GetNamespace().StartsWith(remaining) || id.GetPath().StartsWith(remaining)) {
					builder.Suggest(id.ToString());
				}
			}

			return builder.BuildFuture();
		}

		public override Block Parse(IStringReader reader) {
			int cursor = reader.Cursor;
			string s = reader.ReadString();

			if (!Identifier.TryFromCommandInput(reader, out Identifier identifier)) {
				reader.Cursor = cursor;
				throw CommandSyntaxException.BuiltInExceptions.ReaderExpectedSymbol().CreateWithContext(reader, s);
			}

			RegistryKey<Block> key = RegistryKey<Block>.Of(Registries.BLOCKS.GetKey(), identifier);
			if (!Registries.BLOCKS.TryGet(key, out Block block)) {
				reader.Cursor = cursor;
				throw CommandSyntaxException.BuiltInExceptions.DispatcherUnknownArgument().CreateWithContext(reader);
			}

			return block;
		}
	}
}
