namespace SoulboundBackend.Core.Resource {
	public interface IResourceModule {
		protected static TAsset Resource<TAsset, TGroup>(string name)
				where TAsset : UnityEngine.Object
				where TGroup : ResourceGroups.IResourceGroupDefinition<TAsset> {
			return ResourceManager.Get<TAsset, TGroup>(name);
		}
	}
}
