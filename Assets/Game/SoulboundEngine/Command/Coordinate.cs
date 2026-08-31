namespace SoulboundEngine.Command {
	public readonly struct Coordinate {
		public readonly bool isRelative;
		public readonly bool useTarget;
		public readonly double value;

		public Coordinate(bool isRelative, double value, bool useTarget) {
			this.isRelative = isRelative;
			this.value = value;
			this.useTarget = useTarget;
		}

		public readonly double GetPos(double relative, double target) {
			return this.isRelative
				? this.useTarget
					? target + this.value
					: relative + this.value
				: this.value;
		}
	}
}
