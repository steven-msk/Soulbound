#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public interface IValueRule {
		void Apply(ref double value);

		public static Range Ranged(double minIncluded = double.MinValue, double maxIncluded = double.MaxValue) {
			return new Range(minIncluded, maxIncluded);
		}

		public record Range(double minIncluded, double maxIncluded) : IValueRule {
			public void Apply(ref double value) {
				if (value < minIncluded) value = minIncluded;
				if (value > maxIncluded) value = maxIncluded; 
			}
		}
	}
}
