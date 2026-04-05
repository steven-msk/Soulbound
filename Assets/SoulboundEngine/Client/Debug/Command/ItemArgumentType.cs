using Brigadier.NET;
using Brigadier.NET.ArgumentTypes;
using Brigadier.NET.Context;
using Brigadier.NET.Exceptions;
using Brigadier.NET.Suggestion;
using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Core;
using SoulboundEngine.Core.Registry;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.Debug.Commands {
	public class ItemArgumentType : ArgumentType<Item> {
		public override Task<Suggestions> ListSuggestions<TSource>(CommandContext<TSource> context, SuggestionsBuilder builder) {
			foreach (var item in Registry<Item>.GetAll()) {
				if (item.GetIdentifier().IsPartiallyMatching(builder.RemainingLowerCase)) {
					builder.Suggest(item.GetIdentifier().ToString());
				}
			}

			return builder.BuildFuture();
		}

		public override Item Parse(IStringReader reader) {
			int cursor = reader.Cursor;
			string token = reader.ReadString();

			if (!Identifier.TryFromString(token, out Identifier identifier)) {
				reader.Cursor = cursor;
				throw CommandSyntaxException.BuiltInExceptions.ReaderExpectedSymbol().CreateWithContext(reader, token);
			}
			if (!Registry<Item>.TryGet(identifier, out Item item)) {
				reader.Cursor = cursor;
				throw CommandSyntaxException.BuiltInExceptions.DispatcherUnknownArgument().CreateWithContext(reader);
			}

			return item;
		}
	}
}
