using SoulboundEngine.Core.Registry;

#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public abstract class AttributeType : IIdentifierProvider {
		private readonly Identifier identifier;
		private readonly IValueRange? defaultRange;

		public AttributeType(Identifier identifier, IValueRange? defaultRange) {
			this.identifier = identifier;
			this.defaultRange = defaultRange;
		}

		public Identifier GetIdentifier() => identifier;

		public IValueRange? GetAttributeValueRange() => defaultRange;
	}

	public sealed class AttributeType<T> : AttributeType {
		public AttributeType(Identifier identifier, IValueRange<T>? defaultRange)
			: base(identifier, defaultRange) {
		}
	}
}
