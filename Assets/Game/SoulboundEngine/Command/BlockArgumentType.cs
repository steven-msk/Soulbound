namespace SoulboundEngine.Command {
	using Brigadier.NET;
	using Brigadier.NET.ArgumentTypes;
	using Brigadier.NET.Context;
	using Brigadier.NET.Exceptions;
	using Brigadier.NET.Suggestion;
	using SoulboundEngine.Common;
	using SoulboundEngine.Registry;
	using SoulboundEngine.World.Block;
	using System.Threading.Tasks;

#nullable enable

	public class BlockArgumentType : ArgumentType<Block> {
		public override Task<Suggestions> ListSuggestions<TSource>(CommandContext<TSource> context, SuggestionsBuilder builder) {
			string remaining = builder.RemainingLowerCase;
			return new IdentifierArgumentType(
				id => id.GetNamespace().StartsWith(remaining) || id.GetPath().StartsWith(remaining),
				Registries.BLOCKS.GetIdentifiers
			).ListSuggestions(context, builder);
		}

		public override Block Parse(IStringReader reader) {
			Identifier identifier = new IdentifierArgumentType().Parse(reader);
			RegistryEntry<Block>? block = Registries.BLOCKS.GetEntry(identifier);
			return block == null
				? throw new DynamicCommandExceptionType(o => new LiteralMessage("Invalid block '{}'".WithArgs(o))).CreateWithContext(reader, identifier)
				: block.GetValue();
		}
	}
}
