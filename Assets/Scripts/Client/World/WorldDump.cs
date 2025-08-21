using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct WorldDump {
	public WorldChunk[] generatedChunks;

	public WorldDump(WorldChunk[] generatedChunks) {
		this.generatedChunks = generatedChunks;
	}
}
