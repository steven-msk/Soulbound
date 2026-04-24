using SoulboundEngine.Client.World.EntitySystem.Transform;
using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Core.Registry;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem {
	public abstract class EntityDescriptor {
		public static EntityDescriptor<E> Of<E>(EntityDescriptor<E>.EntityFactory factory, ITransformSupplier<E> transformSupplier) where E : Entity {
			return new EntityDescriptor<E>(factory, transformSupplier);
		}

		public static Identifier GetIdentifier(EntityDescriptor descriptor) {
			return Registries.ENTITIES.GetIdentifier(descriptor);
		}

		public abstract Entity CreateBoxed(Level level, Vector2 pos);
		public abstract IEntityTransform CreateTransform(Entity entity);
	}

	public class EntityDescriptor<E> : EntityDescriptor where E : Entity {
		public delegate E EntityFactory(EntityDescriptor<E> descriptor, Level level);

		private readonly EntityFactory factory;
		private readonly ITransformSupplier<E> transformSupplier;

		public EntityDescriptor(EntityFactory factory, ITransformSupplier<E> transformSupplier) {
			this.factory = factory;
			this.transformSupplier = transformSupplier;
		}

		public E Create(Level level, Vector2 pos) {
			E entity = factory(this, level);
			level.AddEntity(entity);
			entity.SetPos(pos);
			return entity;
		}

		public override IEntityTransform CreateTransform(Entity entity) {
			IEntityTransform transform = transformSupplier.GetTransform((E)entity);
			transform.Bind(entity);
			return transform;
		}

		public override Entity CreateBoxed(Level level, Vector2 pos) {
			return Create(level, pos);
		}

		public override string ToString() => GetIdentifier(this).ToString();
	}
}
