namespace SoulboundEngine.Core.Noise {
	public interface IDomainWarp {
		void DomainWarp(ref float x, ref float y);
		void DomainWarp(ref float x, ref float y, ref float z);

		void SetDomainWarpAmp(float domainWarpAmp);
		void SetDomainWarpType(DomainWarpType domainWarpType);
	}
}
