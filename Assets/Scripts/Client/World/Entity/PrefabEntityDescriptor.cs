using SoulboundBackend.Core.AssetManagement;
using SoulboundBackend.Core.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.World.EntitySystem {
	public class PrefabEntityDescriptor : EntityDescriptor {
		private readonly AssetKey assetKey;
		private Func<AssetKey, GameObject> resourceSelector;

		public PrefabEntityDescriptor(string id, string name, AssetKey assetKey, Func<AssetKey, GameObject> resourceSelector)
			: base(id, name) {
			this.assetKey = assetKey;
			this.resourceSelector = resourceSelector;
		}

		public PrefabEntityDescriptor(string id, string name, AssetKey  assetKey)
			: this(id, name, assetKey, AssetManager.Resolve<GameObject>) {
		}

		public override Entity_OLD CreateInstance() {
			var prefab = resourceSelector(assetKey);
			var obj = GameObject.Instantiate(prefab);
			return obj.GetComponent<Entity_OLD>();
		}
	}
}
