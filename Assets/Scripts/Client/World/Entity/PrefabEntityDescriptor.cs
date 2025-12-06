using SoulboundBackend.Core.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.World.EntitySystem {
	public class PrefabEntityDescriptor : EntityDescriptor {
		private readonly string prefabId;
		private Func<string, GameObject> resourceSelector;

		public PrefabEntityDescriptor(string id, string name, string prefabId, Func<string, GameObject> resourceSelector)
			: base(id, name) {
			this.prefabId = prefabId;
			this.resourceSelector = resourceSelector;
		}

		public PrefabEntityDescriptor(string id, string name, string prefabId)
			: this(id, name, prefabId, ResourceManager.Get<GameObject, ResourceGroups.Prefabs>) {
		}

		public override Entity CreateInstance() {
			var prefab = resourceSelector(prefabId);
			var obj = GameObject.Instantiate(prefab);
			return obj.GetComponent<Entity>();
		}
	}
}
