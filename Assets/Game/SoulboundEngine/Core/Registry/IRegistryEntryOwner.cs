namespace SoulboundEngine.Core.Registry {
	public interface IRegistryEntryOwner<T> {
		public bool OwnerEquals(IRegistryEntryOwner<T> other);
	}
}
