namespace SoulboundEngine.UnityClient.Debug.Commands {
	public struct Coordinate {
		public bool isRelative;
		public double value;

		public readonly double GetPos(double relative) {
			return this.isRelative ? relative + this.value : this.value;
		}
	}
}
