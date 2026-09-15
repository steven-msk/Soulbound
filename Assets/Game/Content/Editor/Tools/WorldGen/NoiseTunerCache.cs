namespace SoulboundEngine.UnityClient.Editor.Tools.WorldGen {
	using System.Collections.Generic;
	using UnityEngine;

	public class NoiseTunerCache : ScriptableObject {
		public int chunkCount = 1;
		public long seed;
		public List<NoiseOverrideEntry> overrideEntries = new();
	}
}
