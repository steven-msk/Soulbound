namespace SoulboundEngine.UnityClient.Debug.Metrics {
	using System;

	public class AverageCounter {
		private readonly int averageThreshold;
		private float min = float.MaxValue;
		private float max = float.MinValue;
		private float average;
		private int count;
		private float currentAverage;
		private float current;
		private bool hasTickedOnce;

		public AverageCounter(int averageThreshold) => this.averageThreshold = averageThreshold;

		public void Tick(float value) {
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

		public float GetAverage() => this.currentAverage;

		public float GetMin() => this.hasTickedOnce ? this.min : 0f;

		public float GetMax() => this.hasTickedOnce ? this.max : 0f;

		public float GetCurrent() => this.current;
	}
}
