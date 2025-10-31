using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.World.Entity {
	[RequireComponent(typeof(Collider2D))]
	public sealed class ManagedEntity : Entity {
		public override Type entityScriptType => typeof(ManagedEntity);

		[Obsolete] public override string prefabDefinitionID => null;

		public override float facing { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public override void ApplySerializedProperties(SerializedEntityPropertyList properties) {
		}

		public override void EntityUpdate(float deltaTime) {
		}

		public override Bounds GetBounds() {
			return this.GetComponent<Collider2D>().bounds;
		}

		public override SerializedEntityPropertyList GetSerializedProperties() {
			return SerializedEntityPropertyList.Empty();
		}

		public override void OnChunkLoaded() {
		}

		public override void OnChunkUnloaded() {
		}
	}
}
