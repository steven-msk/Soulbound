using SoulboundBackend.Core.AssetManagement;

namespace SoulboundBackend.Core.Resource {
	public interface IResourceModule {
		protected static TAsset Resource<TAsset>(AssetKey assetKey) where TAsset : UnityEngine.Object {
			return ResourceManager.GetAddressableSync<TAsset>(assetKey);
		}
	}
}
