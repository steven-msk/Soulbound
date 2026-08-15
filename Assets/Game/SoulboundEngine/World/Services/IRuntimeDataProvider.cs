namespace SoulboundEngine.World.Services {
	public interface IRuntimeDataProvider {
		IRuntimePlayerDataProvider Player { get; }
		IRuntimeEntityDataProvider Entities { get; }
	}
}
