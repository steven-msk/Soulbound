namespace SoulboundEngine.UnityClient.Debug {
	using UnityEngine;

	public readonly struct Line {
		public readonly Vector3 from;
		public readonly Vector3 to;
		public readonly Color color;

		public Line(Vector3 from, Vector3 to) 
			: this(from, to, Color.white) {
		}

		public Line(Vector3 from, Vector3 to, Color color) {
			this.from = from;
			this.to = to;
			this.color = color;
		}
	}
}
