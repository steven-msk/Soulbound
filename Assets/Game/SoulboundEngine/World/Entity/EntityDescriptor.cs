namespace SoulboundEngine.World.Entity {
	using SoulboundEngine.Common.Math;
	using SoulboundEngine.Registry;
	using SoulboundEngine.Serialization;
	using SoulboundEngine.World.Entity.Attribute;
	using SoulboundEngine.World.Level;

#nullable enable

	public abstract class EntityDescriptor {
		public static readonly Codec<EntityDescriptor> CODEC = Identifier.CODEC.FlatXmap(
			decode: id => Get(id) is EntityDescriptor descriptor
				? DataResult<EntityDescriptor>.Success(descriptor)
				: DataResult<EntityDescriptor>.Error($"Unknown entity type: {id}"),
			encode: GetIdentifier
		);

		public static Identifier? GetIdentifier(EntityDescriptor descriptor) {
			return Registries.ENTITIES.GetIdentifier(descriptor);
		}

		public static EntityDescriptor? Get(Identifier id) {
			return Registries.ENTITIES.GetEntry(id)?.GetValue();
		}

		public abstract EntityDimensions GetDimensions();

		public abstract Entity? CreateBoxed(Level level, Vec2d pos);

		public abstract EntityCategory GetCategory();

		public abstract AttributeSupplier GetAttributes();

		public Entity? Create(Level level) {
			return this.CreateBoxed(level, Vec2d.ZERO);
		}
	}

	public class EntityDescriptor<E> : EntityDescriptor where E : Entity {
		public delegate E? EntityFactory(EntityDescriptor<E> descriptor, Level level);
		private readonly EntityDimensions dimensions;
		private readonly EntityCategory category;
		private readonly AttributeSupplier attributes;
		private readonly EntityFactory factory;

		protected EntityDescriptor(EntityFactory factory, EntityDimensions dimensions, EntityCategory category, AttributeSupplier attributes) {
			this.factory = factory;
			this.dimensions = dimensions;
			this.category = category;
			this.attributes = attributes;
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

		public override EntityCategory GetCategory() => this.category;

		public override string ToString() => GetIdentifier(this).ToString();

		public override EntityDimensions GetDimensions() => this.dimensions;

		public override AttributeSupplier GetAttributes() => this.attributes;

		public sealed class Builder {
			private EntityDimensions dimensions = EntityDimensions.Scalable(Entity.DEFAULT_BB_WIDTH, Entity.DEFAULT_BB_HEIGHT);
			private readonly EntityFactory factory;
			private readonly EntityCategory category;
			private readonly AttributeSupplier attributes;

			private Builder(EntityFactory factory, EntityCategory category, AttributeSupplier attributes) {
				this.factory = factory;
				this.category = category;
				this.attributes = attributes;
			}

			public static Builder Of(EntityCategory category, EntityFactory factory, AttributeSupplier attributes) {
				return new Builder(factory, category, attributes);
			}

			public static Builder OfNothing(EntityCategory category, AttributeSupplier attributes) {
				return new Builder((_, _) => null, category, attributes);
			}

			public Builder Sized(double width, double height) {
				this.dimensions = EntityDimensions.Scalable(width, height);
				return this;
			}

			public EntityDescriptor<E> Build() {
				return new EntityDescriptor<E>(this.factory, this.dimensions, this.category, this.attributes);
			}
		}
	}
}
