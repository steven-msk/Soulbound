namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public interface IAttributeModifier {
		void Apply(ref object value);
		bool HasPredicate();
	}

	public interface IAttributeModifier<T> : IAttributeModifier {
		void Apply(ref T value);

		void IAttributeModifier.Apply(ref object value) {
			T wrap = (T)value;
			Apply(ref wrap);
			value = wrap;
		}
	}
}
