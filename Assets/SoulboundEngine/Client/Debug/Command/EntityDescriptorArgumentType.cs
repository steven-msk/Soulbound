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
			foreach (var descriptor in Registry<EntityDescriptor>.GetAll()) {
				if (descriptor.GetIdentifier().IsPartiallyMatching(builder.RemainingLowerCase)) {
					builder.Suggest(descriptor.GetIdentifier().ToString());
				}
			}

			return builder.BuildFuture();
		}

		public override EntityDescriptor Parse(IStringReader reader) {
			int cursor = reader.Cursor;
			string token = reader.ReadString();

			if (!Identifier.TryFromString(token, out var identifier)) {
				reader.Cursor = cursor;
				throw CommandSyntaxException.BuiltInExceptions.ReaderExpectedSymbol().CreateWithContext(reader, token);
			}
			if (!Registry<EntityDescriptor>.TryGet(identifier, out EntityDescriptor descriptor)) {
				reader.Cursor = cursor;
				throw CommandSyntaxException.BuiltInExceptions.DispatcherUnknownArgument().CreateWithContext(reader);
			}

			return descriptor;
		}
	}
}
