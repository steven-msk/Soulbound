namespace SoulboundEngine.World.Gen.Feature {
	using SoulboundEngine.Registry;
	using System.Collections.Generic;

	public interface IFeatureConfig {
		public virtual IEnumerable<RegistryEntry<ConfiguredFeature>> GetSubFeatures() {
			yield break;
		}
	}
}
