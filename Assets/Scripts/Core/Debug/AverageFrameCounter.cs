using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core.Debug {
	public class AverageFrameCounter {
		private readonly int averageCount;
		private float average;
		private int frameCount;
		private float currentAverage;

		public AverageFrameCounter(int averageCount) => this.averageCount = averageCount;

		public void Tick(float value) {
			average += value;
			frameCount++;

			if (frameCount >= averageCount) {
				currentAverage = average / averageCount;
				frameCount = 0;
				average = 0f;
			}
		}

		public float GetAverage() => currentAverage;
	}
}
