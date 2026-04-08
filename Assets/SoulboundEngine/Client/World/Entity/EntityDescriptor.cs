using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Core.Registry;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem {
	public class EntityDescriptor : IIdentifierProvider {
		public delegate Entity EntityFactory(EntityDescriptor descriptor, Level level);

		private readonly Identifier identifier;
		private readonly EntityFactory factory;

		public EntityDescriptor(Identifier identifier, EntityFactory factory) {
			this.identifier = identifier;
			this.factory = factory;
		}

		public Identifier GetIdentifier() => identifier;

		public Entity Create(Level level, Vector2 pos) {
			Entity entity = factory(this, level);
			level.AddEntity(entity);
			entity.SetPos(pos);
			return entity;
		}

		public override string ToString() => identifier.ToString();

	}
}
