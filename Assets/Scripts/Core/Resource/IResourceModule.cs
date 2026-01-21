using SoulboundBackend.Core.AssetManagement;

namespace SoulboundBackend.Core.Resource {
	public interface IResourceModule {
		protected static TAsset Resource<TAsset, TGroup>(AssetKey assetKey)
				where TAsset : UnityEngine.Object
				where TGroup : IResourceGroupDefinition<TAsset> {
			return ResourceManager.GetAddressableSync<TAsset>(assetKey);
		}
	}
}
