namespace SoulboundEngine.UnityClient.Debug.Metrics {
	using System;

	public class AverageCounter {
		private readonly int averageThreshold;
		private double min = double.MaxValue;
		private double max = double.MinValue;
		private double average;
		private int count;
		private double currentAverage;
		private double current;
		private bool hasTickedOnce;

		public AverageCounter(int averageThreshold) => this.averageThreshold = averageThreshold;

		public void Tick(double value) {
			this.average += value;
			this.current = value;
			this.count++;
			this.min = Math.Min(this.min, value);
			this.max = Math.Max(this.max, value);
			this.hasTickedOnce = true;

			if (this.count >= this.averageThreshold) {
				this.currentAverage = this.average / this.averageThreshold;
				this.count = 0;
				this.average = 0f;
			}
		}

		public void Tick(int value) => this.Tick((float)value);

		public void Tick(float value) => this.Tick((double)value);

		public double GetAverage() => this.currentAverage;

		public double GetMin() => this.hasTickedOnce ? this.min : 0.0d;

		public double GetMax() => this.hasTickedOnce ? this.max : 0.0d;

		public double GetCurrent() => this.current;
	}
}
