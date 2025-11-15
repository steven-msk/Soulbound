using SoulboundBackend.Client.World.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace SoulboundBackend.Client.Combat {
	public class AttackContext {
		public readonly Entity performer;
		public readonly AttackSource source;
		public AttackAnimationHandler animationHandler;
		public AttackEventDispatcher eventDispatcher;
		public object metadata { get; init; }
		private List<GameObject> tempObjects = new();

		public AttackContext(Entity performer, AttackSource source) {
			this.performer = performer;
			this.source = source;
		}

		public void AddTemp(GameObject obj) => tempObjects.Add(obj);

		public void AddTemp(params GameObject[] objects) => tempObjects.AddRange(objects);

		public void RemoveTemp(GameObject obj) => tempObjects.Remove(obj);

		public void RemoveTemp(params GameObject[] objects) {
			foreach (var obj in objects) {
				RemoveTemp(obj);
			}
		}

		public void DestroyAllTemp() {
			var toRemove = new List<GameObject>();
			tempObjects.ForEach(toRemove.Add);
			tempObjects.Clear();
			toRemove.ForEach(GameObject.Destroy);
		}

		public void DestroyTemp(GameObject obj) {
			tempObjects.Remove(obj);
			GameObject.Destroy(obj);
		}
	}
}
