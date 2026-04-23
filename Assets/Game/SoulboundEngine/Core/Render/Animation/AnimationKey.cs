namespace SoulboundEngine.Core.Render.Animation {
	public readonly struct AnimationKey {
		public readonly string value;

		public AnimationKey(string key) {
			this.value = key;
		}

		public override bool Equals(object obj) {
			return obj is AnimationKey key && this.value == key.value;
		}

		public override int GetHashCode() {
			return value.GetHashCode();
		}

		public override string ToString() => value;
	}
}
