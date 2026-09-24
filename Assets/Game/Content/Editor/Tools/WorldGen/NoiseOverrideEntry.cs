namespace SoulboundEngine.UnityClient.Editor.Tools.WorldGen {
	using System;

	[Serializable]
	public struct NoiseOverrideEntry {
		public string registryKey;
		public int firstOctave;
		public double[] octaveMultipliers;
	}
}
