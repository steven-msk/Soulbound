using SoulboundEngine.Core.Registry;
using System;
using UnityEngine;

namespace SoulboundEngine.Client.World.EntitySystem {
	public class EntityDescriptor : IIdentifierProvider {
		private readonly Identifier identifier;
		private readonly Func<Vector2, Entity> factory;

		public EntityDescriptor(Identifier identifier, Func<Vector2, Entity> factory) {
			this.identifier = identifier;
			this.factory = factory;
		}

		public Identifier GetIdentifier() => identifier;

		public Entity Create(Vector2 pos) => factory(pos);

		public override string ToString() => identifier.ToString();
	}
}
