using SoulboundEngine.Client.World.EntitySystem.Transform;
using SoulboundEngine.Common.Unity;
using SoulboundEngine.Core;
using SoulboundEngine.Core.Assets;
using SoulboundEngine.Core.Registry;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem {
	public static class EntityType {

		// TODO: move out transform supplier logic into Unity adapters layer

		public static readonly EntityDescriptor<MovingEntity> MOVING_ENTITY = Register(
			"moving_entity",
			(descriptor, level) => new MovingEntity(descriptor, level),
			ITransformSupplier<MovingEntity>.Of(entity => {
				GameObject obj = new("Static Entity", typeof(StaticTransform));

				Sprite sprite = AssetManager.Resolve<Sprite>(new AssetKey("WhiteSquare"));
				obj.AddComponent<SpriteRenderer>().sprite = sprite;

				return obj.GetComponent<StaticTransform>();
			})
		);
		public static readonly EntityDescriptor<StaticEntity> STATIC_ENTITY = Register(
			"static_entity",
			(descriptor, level) => new StaticEntity(descriptor, level),
			ITransformSupplier<StaticEntity>.Of(entity => {
				GameObject obj = new("Static Entity", typeof(StaticTransform));

				Sprite sprite = AssetManager.Resolve<Sprite>(new AssetKey("WhiteSquare"));
				obj.AddComponent<SpriteRenderer>().sprite = sprite;

				return obj.GetComponent<StaticTransform>();
			})
		);
		public static readonly EntityDescriptor<PhysicsEntity> PHYSICS_ENTITY = Register(
			"physics_entity",
			(descriptor, level) => new PhysicsEntity(descriptor, level),
			ITransformSupplier<PhysicsEntity>.Of(entity => {
				GameObject obj = new("Physics Entity", typeof(PhysicsTransform));

				Sprite sprite = AssetManager.Resolve<Sprite>(new AssetKey("WhiteSquare"));
				SpriteRenderer spriteRenderer = obj.AddComponent<SpriteRenderer>();
				spriteRenderer.sprite = sprite;
				spriteRenderer.color = entity.GetSpriteColor();

				obj.GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Continuous;
				obj.AddComponent<BoxCollider2D>();

				PhysicsTransform transform = obj.GetComponent<PhysicsTransform>();
				transform.SetCollisionHandler(entity);

				return transform;
			})
		);
		public static readonly EntityDescriptor<AreaTriggerEntity> AREA_TRIGGER_ENTITY = Register(
			"area_trigger_entity",
			(descriptor, level) => new AreaTriggerEntity(descriptor, level),
			ITransformSupplier<AreaTriggerEntity>.Of(entity => {
				GameObject obj = new("Area Trigger Entity");

				BoxCollider2D collider = obj.AddComponent<BoxCollider2D>();
				collider.size = new Vector2(4f, 0.15f);
				collider.isTrigger = true;
				collider.excludeLayers = ~LayerMask.GetMask(Layers.EntityCharacter);

				TriggerCollisionListener triggerListener = obj.AddComponent<TriggerCollisionListener>();
				triggerListener.onTriggerEnter += entity.OnAreaEnter;
				triggerListener.onTriggerExit += entity.OnAreaExit;

				return obj.AddComponent<StaticTransform>();
			})
		);

		private static EntityDescriptor<E> Register<E>(string id, EntityDescriptor<E> descriptor) where E : Entity {
			return Registry<EntityDescriptor>.Register(Registries.ENTITIES, KeyOf(id), descriptor);
		}

		private static EntityDescriptor<E> Register<E>(string id, EntityDescriptor<E>.EntityFactory factory, ITransformSupplier<E> transformSupplier) where E : Entity {
			return Register(id, EntityDescriptor.Of(factory,transformSupplier));
		}

		private static RegistryKey<EntityDescriptor> KeyOf(string id) {
			return RegistryKey<EntityDescriptor>.Of(Registries.ENTITIES.GetKey(), Identifier.Of(id));
		}

		public static void Init() { }
	}
}
