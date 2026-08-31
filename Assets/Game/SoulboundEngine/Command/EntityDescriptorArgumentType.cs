namespace SoulboundEngine.Command {
	using Brigadier.NET;
	using Brigadier.NET.ArgumentTypes;
	using Brigadier.NET.Context;
	using Brigadier.NET.Exceptions;
	using Brigadier.NET.Suggestion;
	using SoulboundEngine.Common;
	using SoulboundEngine.Registry;
	using SoulboundEngine.World.Entity;
	using System.Linq;
	using System.Threading.Tasks;

	public class EntityDescriptorArgumentType : ArgumentType<EntityDescriptor> {
		public override Task<Suggestions> ListSuggestions<TSource>(CommandContext<TSource> context, SuggestionsBuilder builder) {
			string remaining = builder.RemainingLowerCase;
			return new IdentifierArgumentType(
				id => id.GetNamespace().StartsWith(remaining) || id.GetPath().StartsWith(remaining),
				() => Registries.ENTITIES.Where(e => e.CanSpawnByCommand()).Select(EntityDescriptor.GetIdentifier)
			).ListSuggestions(context, builder);
		}

		public override EntityDescriptor Parse(IStringReader reader) {
			Identifier identifier = new IdentifierArgumentType().Parse(reader);
			RegistryEntry<EntityDescriptor> entity = Registries.ENTITIES.GetEntry(identifier);
			return entity == null || !entity.GetValue().CanSpawnByCommand()
				? throw new DynamicCommandExceptionType(o => new LiteralMessage("Unknown entity '{}'".WithArgs(o))).CreateWithContext(reader, identifier)
				: entity.GetValue();
		}
	}
}
