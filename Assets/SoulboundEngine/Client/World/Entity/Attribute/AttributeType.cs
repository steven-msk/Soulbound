using SoulboundEngine.Core.Registry;

#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public abstract class AttributeType : IIdentifierProvider {
		protected readonly Identifier identifier;
		protected readonly IValueRule? valueRule;

		public AttributeType(Identifier identifier, IValueRule? valueRule) {
			this.identifier = identifier;
			this.valueRule = valueRule;
		}

		public Identifier GetIdentifier() => identifier;
		public IValueRule? GetValueRule() => valueRule;
	}

	public sealed class AttributeType<T> : AttributeType {
		public AttributeType(Identifier identifier, IValueRule<T>? valueRule)
			: base(identifier, valueRule) {
		}

		public new IValueRule<T>? GetValueRule() => (IValueRule<T>?)valueRule;
	}
}
