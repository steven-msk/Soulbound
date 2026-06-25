using SoulboundEngine.Client.World.Level;
using SoulboundEngine.Core.Registry;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.World.Entity {
	public abstract class EntityDescriptor {
		public static EntityDescriptor<E> Of<E>(EntityDescriptor<E>.EntityFactory factory) where E : Entity {
			return new EntityDescriptor<E>(factory);
		}

		public static Identifier GetIdentifier(EntityDescriptor descriptor) {
			return Registries.ENTITIES.GetIdentifier(descriptor);
		}

		public abstract Entity CreateBoxed(Level.Level level, Vector2 pos);
	}

	public class EntityDescriptor<E> : EntityDescriptor where E : Entity {
		public delegate E EntityFactory(EntityDescriptor<E> descriptor, Level.Level level);

		private readonly EntityFactory factory;

		public EntityDescriptor(EntityFactory factory) {
			this.factory = factory;
		}

		public E Create(Level.Level level, Vector2 pos) {
			E entity = this.factory(this, level);
			level.AddEntity(entity);
			entity.SetPosition(pos);
			return entity;
		}

		public override Entity CreateBoxed(Level.Level level, Vector2 pos) {
			return this.Create(level, pos);
		}

		public override string ToString() => GetIdentifier(this).ToString();
	}
}
