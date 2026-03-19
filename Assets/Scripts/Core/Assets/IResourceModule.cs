namespace SoulboundBackend.Core.Assets {
	public interface IResourceModule {
		protected static TAsset Resource<TAsset>(AssetKey assetKey) where TAsset : UnityEngine.Object {
			return AssetManager.Resolve<TAsset>(assetKey);
		}
	}
}
