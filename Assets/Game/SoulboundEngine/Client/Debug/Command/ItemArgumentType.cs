using Brigadier.NET;
using Brigadier.NET.ArgumentTypes;
using Brigadier.NET.Context;
using Brigadier.NET.Exceptions;
using Brigadier.NET.Suggestion;
using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Core.Registry;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.Debug.Commands {
	public class ItemArgumentType : ArgumentType<Item> {
		public override Task<Suggestions> ListSuggestions<TSource>(CommandContext<TSource> context, SuggestionsBuilder builder) {
			string remaining = builder.RemainingLowerCase;

			foreach (var item in Registries.ITEMS) {
				Identifier id = Items.GetIdentifier(item);

				if (id.GetNamespace().StartsWith(remaining) || id.GetPath().StartsWith(remaining)) {
					builder.Suggest(id.ToString());
				}
			}

			return builder.BuildFuture();
		}

		public override Item Parse(IStringReader reader) {
			int cursor = reader.Cursor;

			if (!Identifier.TryFromCommandInput(reader, out var identifier)) {
				reader.Cursor = cursor;
				throw CommandSyntaxException.BuiltInExceptions.ReaderExpectedSymbol().Create(reader);
			}

			RegistryKey<Item> key = RegistryKey<Item>.Of(Registries.ITEMS.GetKey(), identifier);
			if (!Registries.ITEMS.TryGet(key, out Item item)) {
				reader.Cursor = cursor;
				throw CommandSyntaxException.BuiltInExceptions.DispatcherUnknownArgument().CreateWithContext(reader);
			}

			return item;
		}
	}
}
