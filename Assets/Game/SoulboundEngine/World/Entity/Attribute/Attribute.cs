namespace SoulboundEngine.World.Entity.Attribute {
	public class Attribute {
		private Impact impact = Impact.POSITIVE;

		protected Attribute(string descriptionId, double defaultValue) {
			this.descriptionId = descriptionId;
			this.defaultValue = defaultValue;
		}

		public double defaultValue { get; private set; }
		public string descriptionId { get; private set; }

		public Attribute SetImpact(Impact impact) {
			this.impact = impact;
			return this;
		}

		public virtual double ValidateValue(double value) => value;

		public Impact GetImpact() => this.impact;

		public enum Impact {
			POSITIVE,
			NEUTRAL,
			NEGATIVE
		}
	}
}
