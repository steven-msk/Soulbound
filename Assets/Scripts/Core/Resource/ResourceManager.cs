#nullable enable

public static class ResourceManager {

	public static TAsset? Get<TAsset, TGroup>(string name)
			where TAsset : UnityEngine.Object
			where TGroup : ResourceGroups.IResourceGroupDefinition<TAsset> {
		string address = ResourceGroups.GetAddressFromGroupDefinition<TGroup>();
		ResourceGroup group = ResourceGroups.GetGroupByAddress(address);
		return group.GetAsset<TAsset>(name);
	}
}
