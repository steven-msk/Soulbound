namespace SoulboundEngine.World.Entity.Attribute {
	using System;

	public class RangedAttribute : Attribute {
		public double minValue { get; private set; }
		public double maxValue { get; private set; }

		public RangedAttribute(string descriptionId, double defaultValue, double minValue, double maxValue)
			: base(descriptionId, defaultValue) {
			this.minValue = minValue;
			this.maxValue = maxValue;
			if (minValue > maxValue) throw new ArgumentException("Min value cannot be bigger than max value");
			if (defaultValue < minValue) throw new ArgumentException("Default value cannot be less than min value");
			if (defaultValue > maxValue) throw new ArgumentException("Default value cannot be more than max value");
		}

		public override double ValidateValue(double value) {
			return double.IsNaN(value) ? this.minValue : Math.Clamp(value, this.minValue, this.maxValue);
		}
	}
}
