namespace SoulboundEngine.Command {
	using Brigadier.NET;
	using Brigadier.NET.ArgumentTypes;
	using Brigadier.NET.Context;
	using Brigadier.NET.Exceptions;
	using Brigadier.NET.Suggestion;
	using SoulboundEngine.Common;
	using SoulboundEngine.Item;
	using SoulboundEngine.Registry;
	using System.Threading.Tasks;
	using Item = Item.Item;

#nullable enable

	public class ItemArgumentType : ArgumentType<Item> {
		public override Task<Suggestions> ListSuggestions<TSource>(CommandContext<TSource> context, SuggestionsBuilder builder) {
			string remaining = builder.RemainingLowerCase;

			foreach (Item item in Registries.ITEMS) {
				if (item == Items.AIR) continue;
				Identifier id = Items.GetIdentifier(item);

				if (id.GetNamespace().StartsWith(remaining) || id.GetPath().StartsWith(remaining)) {
					builder.Suggest(id.ToString());
				}
			}

			return builder.BuildFuture();
		}

		public override Item Parse(IStringReader reader) {
			if (!Identifier.TryFromCommandInput(reader, out Identifier identifier)) {
				throw new SimpleCommandExceptionType(new LiteralMessage("Invalid identifier")).CreateWithContext(reader);
			}

			RegistryEntry<Item>? item = Registries.ITEMS.GetEntry(identifier);
			return item == null || item.GetValue() == Items.AIR
				? throw new DynamicCommandExceptionType(o => new LiteralMessage("Unknown item '{}'".WithArgs(o))).CreateWithContext(reader, identifier)
				: item.GetValue();
		}
	}
}
