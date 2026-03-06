using SoulboundBackend.Client.World.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Core {
	public class UpdateManager : MonoBehaviour, IEntitySubsystem {
		private List<IUpdatable> updatables = new();

		public void AddEntity(Entity_OLD entity) {
			if (entity is IUpdatable updatable) {
				updatables.Add(updatable);
			}
		}

		public void RemoveEntity(Entity_OLD entity) {
			if (entity is IUpdatable updatable) {
				updatables.Remove(updatable);
			}
		}

		private void Update() {
			var deltaTime = Time.deltaTime;
			var clone = new List<IUpdatable>(updatables);
			foreach (var updatable in clone) {
				updatable.FrameUpdate(deltaTime);
			}
		}
	}
}
