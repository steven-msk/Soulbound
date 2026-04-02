using SoulboundEngine.Common.Patterns;
using SoulboundEngine.Core.Registry;

namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public abstract class AttributeModifier<T> : IAttributeModifier<T> {
		private readonly RefAction<T> operation;
		private readonly Identifier source;

		public AttributeModifier(Identifier source, RefAction<T> operation) {
			this.source = source;
			this.operation = operation;
		}

		public void Apply(ref T value) {
			operation(ref value);
		}
	}
}
