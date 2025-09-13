using System;

namespace SoulboundBackend.Client.World.Entity {

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	public class EntitySpawnPropertyCandidatesAttribute : Attribute {
		public string[] candidates { get; }

		public EntitySpawnPropertyCandidatesAttribute(params string[] candidates) {
			this.candidates = candidates;
		}
	}
}