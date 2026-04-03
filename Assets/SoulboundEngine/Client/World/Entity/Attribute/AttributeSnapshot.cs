using System.Collections.Generic;

#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public readonly struct AttributeSnapshot<T> {
		public readonly AttributeType<T> attribute;
		public readonly T baseValue;
		public readonly T currentValue;
		public readonly IReadOnlyList<IAttributeModifier<T>> modifiers;
		public readonly IValueRule<T>? valueRule;

		public AttributeSnapshot(AttributeType<T> attribute, T baseValue, T currentValue, IReadOnlyList<IAttributeModifier<T>> modifiers, IValueRule<T>? valueRule) {
			this.baseValue = baseValue;
			this.currentValue = currentValue;
			this.modifiers = modifiers;
			this.valueRule = valueRule;
			this.attribute = attribute;
		}
	}
}
