using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Core.Registry;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem {
	public class EntityDescriptor {
		public delegate Entity EntityFactory(EntityDescriptor descriptor, Level level);

		private readonly EntityFactory factory;

		public EntityDescriptor(EntityFactory factory) {
			this.factory = factory;
		}

		public static Identifier GetIdentifier(EntityDescriptor descriptor) {
			return Registries.ENTITIES.GetIdentifier(descriptor);
		}

		public Entity Create(Level level, Vector2 pos) {
			Entity entity = factory(this, level);
			level.AddEntity(entity);
			entity.SetPos(pos);
			return entity;
		}

		public override string ToString() => GetIdentifier(this).ToString();

	}
}
