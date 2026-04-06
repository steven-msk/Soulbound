using Brigadier.NET;
using Brigadier.NET.ArgumentTypes;
using Brigadier.NET.Context;
using Brigadier.NET.Exceptions;
using Brigadier.NET.Suggestion;
using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Core;
using SoulboundEngine.Core.Registry;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.Debug.Commands {
	public class BlockArgumentType : ArgumentType<Block> {
		public override Task<Suggestions> ListSuggestions<TSource>(CommandContext<TSource> context, SuggestionsBuilder builder) {
			string remaining = builder.RemainingLowerCase;

			foreach (var block in Registry<Block>.GetAll()) {
				Identifier id = block.GetIdentifier();

				if (id.GetNamespace().StartsWith(remaining) || id.GetPath().StartsWith(remaining)) {
					builder.Suggest(id.ToString());
				}
			}

			return builder.BuildFuture();
		}

		public override Block Parse(IStringReader reader) {
			int cursor = reader.Cursor;
			string s = reader.ReadString();

			if (!Identifier.TryFromCommandInput(reader, out var identifier)) {
				reader.Cursor = cursor;
				throw CommandSyntaxException.BuiltInExceptions.ReaderExpectedSymbol().CreateWithContext(reader, s);
			}

			if (!Registry<Block>.TryGet(identifier, out Block block)) {
				reader.Cursor = cursor;
				throw CommandSyntaxException.BuiltInExceptions.DispatcherUnknownArgument().CreateWithContext(reader);
			}

			return block;
		}
	}
}
