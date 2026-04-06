using Brigadier.NET;
using Brigadier.NET.ArgumentTypes;
using Brigadier.NET.Context;
using Brigadier.NET.Exceptions;
using Brigadier.NET.Suggestion;
using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Core;
using SoulboundEngine.Core.Registry;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.Debug.Commands {
	public class ItemArgumentType : ArgumentType<Item> {
		public override Task<Suggestions> ListSuggestions<TSource>(CommandContext<TSource> context, SuggestionsBuilder builder) {
			string remaining = builder.RemainingLowerCase;

			foreach (var item in Registry<Item>.GetAll()) {
				Identifier id = item.GetIdentifier();

				if (id.GetNamespace().StartsWith(remaining) || id.GetPath().StartsWith(remaining)) {
					builder.Suggest(id.ToString());
				}
			}

			return builder.BuildFuture();
		}

		public override Item Parse(IStringReader reader) {
			int cursor = reader.Cursor;

			if (!Identifier.TryParse(reader, out var identifier)) {
				reader.Cursor = cursor;
				throw CommandSyntaxException.BuiltInExceptions.ReaderExpectedSymbol().Create(reader);
			}


			if (!Registry<Item>.TryGet(identifier, out Item item)) {
				Logger.LogInfo("exception: {}", identifier);
				reader.Cursor = cursor;
				throw CommandSyntaxException.BuiltInExceptions.DispatcherUnknownArgument().CreateWithContext(reader);
			}

			return item;
		}
	}
}
