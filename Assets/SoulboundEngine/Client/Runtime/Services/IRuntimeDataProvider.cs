namespace SoulboundEngine.Client.Runtime.Services {
	public interface IRuntimeDataProvider {
		IRuntimePlayerDataProvider Player { get; }
		IRuntimeEntityDataProvider Entities { get; }
	}
}
