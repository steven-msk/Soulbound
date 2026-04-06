using Brigadier.NET;
using Brigadier.NET.ArgumentTypes;
using Brigadier.NET.Context;
using Brigadier.NET.Exceptions;
using Brigadier.NET.Suggestion;
using SoulboundEngine.Client.World.EntitySystem;
using SoulboundEngine.Core;
using SoulboundEngine.Core.Registry;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.Debug.Commands {
	public class EntityDescriptorArgumentType : ArgumentType<EntityDescriptor> {
		public override Task<Suggestions> ListSuggestions<TSource>(CommandContext<TSource> context, SuggestionsBuilder builder) {
			string remaining = builder.RemainingLowerCase;

			foreach (var descriptor in Registry<EntityDescriptor>.GetAll()) {
				Identifier id = descriptor.GetIdentifier();

				if (id.GetNamespace().StartsWith(remaining) || id.GetPath().StartsWith(remaining)) {
					builder.Suggest(id.ToString());
				}
			}

			return builder.BuildFuture();
		}

		public override EntityDescriptor Parse(IStringReader reader) {
			int cursor = reader.Cursor;

			if (!Identifier.TryFromCommandInput(reader, out var identifier)) {
				reader.Cursor = cursor;
				throw CommandSyntaxException.BuiltInExceptions.ReaderExpectedSymbol().Create(reader);
			}

			if (!Registry<EntityDescriptor>.TryGet(identifier, out EntityDescriptor descriptor)) {
				reader.Cursor = cursor;
				throw CommandSyntaxException.BuiltInExceptions.DispatcherUnknownArgument().CreateWithContext(reader);
			}

			return descriptor;
		}
	}
}
