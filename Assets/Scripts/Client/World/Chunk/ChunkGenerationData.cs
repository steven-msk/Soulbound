using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct ChunkGenerationData {
	public Dictionary<int, int> surfaceLevels;
	public int highestUndergroundLevel;

	public ChunkGenerationData(Dictionary<int, int> surfaceLevels, int highestUndergroundLevel) {
		this.surfaceLevels = surfaceLevels;
		this.highestUndergroundLevel = highestUndergroundLevel;
	}
}
