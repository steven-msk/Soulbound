namespace SoulboundBackend.Client.World.EntitySystem.AI {
	public sealed class MutableStateProperty<TType> {
		public TType value { get; private set; }

		public MutableStateProperty(TType value) {
			this.value = value;
		}

		public void MutateValue(TType newValue) {
			this.value = newValue;
		}
	}
}
