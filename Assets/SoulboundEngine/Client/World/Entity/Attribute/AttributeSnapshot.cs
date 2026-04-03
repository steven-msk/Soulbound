#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public readonly struct AttributeSnapshot<T> {
		public readonly AttributeType<T> attribute;
		public readonly T baseValue;
		public readonly T currentValue;

		public AttributeSnapshot(AttributeType<T> attribute, T baseValue, T currentValue) {
			this.baseValue = baseValue;
			this.currentValue = currentValue;
			this.attribute = attribute;
		}
	}
}
