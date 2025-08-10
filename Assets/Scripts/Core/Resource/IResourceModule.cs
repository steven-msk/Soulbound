using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IResourceModule {
	protected static TAsset Resource<TAsset, TGroup>(string name)
			where TAsset : UnityEngine.Object
			where TGroup : ResourceGroups.IResourceGroupDefinition<TAsset> {
		return ResourceManager.Get<TAsset, TGroup>(name);
	}
}
