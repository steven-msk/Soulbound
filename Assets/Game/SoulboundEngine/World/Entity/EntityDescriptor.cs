using SoulboundEngine.Common.Math;
using SoulboundEngine.Registry;

#nullable enable

namespace SoulboundEngine.World.Entity {
	using Level = Level.Level;

	public abstract class EntityDescriptor {
		public static Identifier? GetIdentifier(EntityDescriptor descriptor) {
			return Registries.ENTITIES.GetIdentifier(descriptor);
		}

		public static EntityDescriptor? Get(Identifier id) {
			return Registries.ENTITIES.GetEntry(id)?.GetValue();
		}

		public abstract EntityDimensions GetDimensions();
		public abstract Entity? CreateBoxed(Level level, Vec2d pos);

		public Entity? Create(Level level) {
			return this.CreateBoxed(level, Vec2d.ZERO);
		}
	}

	public class EntityDescriptor<E> : EntityDescriptor where E : Entity {
		public delegate E? EntityFactory(EntityDescriptor<E> descriptor, Level level);

		private readonly EntityDimensions dimensions;
		private readonly EntityFactory factory;

		protected EntityDescriptor(EntityFactory factory, EntityDimensions dimensions) {
			this.factory = factory;
			this.dimensions = dimensions;
		}

		public E? Create(Level level, Vec2d pos) {
			E? entity = this.factory(this, level);
			if (entity == null) return null;

			entity.SetPos(pos);
			return entity;
		}

		public override Entity? CreateBoxed(Level level, Vec2d pos) {
			return this.Create(level, pos);
		}

		public override string ToString() => GetIdentifier(this).ToString();

		public override EntityDimensions GetDimensions() => this.dimensions;
			
		public sealed class Builder {
			private readonly EntityFactory factory;
			private EntityDimensions dimensions = EntityDimensions.Scalable(Entity.DEFAULT_BB_WIDTH, Entity.DEFAULT_BB_HEIGHT);

			private Builder(EntityFactory factory) {
				this.factory = factory;
			}

			public static Builder Of(EntityFactory factory) {
				return new Builder(factory);
			}

			public static Builder OfNothing() {
				return new Builder((_, _) => null);
			}

			public Builder Sized(double width, double height) {
				this.dimensions = EntityDimensions.Scalable(width, height);
				return this;
			}

			public EntityDescriptor<E> Build() {
				return new EntityDescriptor<E>(this.factory, this.dimensions);
			}
		}
	}
}
