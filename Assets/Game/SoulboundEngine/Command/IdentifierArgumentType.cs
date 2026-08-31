namespace SoulboundEngine.Command {
	using Brigadier.NET;
	using Brigadier.NET.ArgumentTypes;
	using Brigadier.NET.Context;
	using Brigadier.NET.Exceptions;
	using Brigadier.NET.Suggestion;
	using SoulboundEngine.Registry;
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	public class IdentifierArgumentType : ArgumentType<Identifier> {
		private readonly Func<IEnumerable<Identifier>> suggestionsSupplier;

		public IdentifierArgumentType(Func<IEnumerable<Identifier>> suggestionsSupplier) {
			this.suggestionsSupplier = suggestionsSupplier;
		}

		public IdentifierArgumentType(Predicate<Identifier> suggestionPredicate, Func<IEnumerable<Identifier>> suggestionsSupplier) 
			: this(() => {
				List<Identifier> identifiers = new();
				foreach (Identifier identifier in suggestionsSupplier()) {
					if (suggestionPredicate(identifier)) {
						identifiers.Add(identifier);
					}
				}
				return identifiers;
			}) {
		}

		public IdentifierArgumentType() 
			: this(() => new List<Identifier>()) {
		}

		public override Task<Suggestions> ListSuggestions<TSource>(CommandContext<TSource> context, SuggestionsBuilder builder) {
			foreach (Identifier identifier in this.suggestionsSupplier()) {
				builder.Suggest(identifier.ToString());			
			}
			return builder.BuildFuture();
		}

		public override Identifier Parse(IStringReader reader) {
			return !Identifier.TryFromCommandInput(reader, out Identifier identifier)
				? throw new SimpleCommandExceptionType(new LiteralMessage("Invalid identifier")).CreateWithContext(reader)
				: identifier;
		}
	}
}
