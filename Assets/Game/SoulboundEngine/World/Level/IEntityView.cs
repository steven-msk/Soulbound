namespace SoulboundEngine.World.Level {
	using SoulboundEngine.World.Entity;
	using SoulboundEngine.World.Physics;
	using SoulboundEngine.World.Player;
	using System;
	using System.Collections.Generic;

#nullable enable

	public interface IEntityView {
		List<Entity> GetEntities(Entity? except, AABB box, Predicate<Entity> selector);

		PlayerEntity GetPlayer();

		public virtual List<AABB> GetEntityCollisions(Entity? source, AABB testBox) {
			if (testBox.GetSize() < 1.0E-7) return new List<AABB>();

			Predicate<Entity> canCollide = source == null ? Entity.CAN_BE_COLLIDED_WITH : e => e.CanBeCollidedWith(source);
			List<Entity> collidingEntities = this.GetEntities(source, testBox.Stretch(1.0E-7), canCollide);
			if (collidingEntities.Count == 0) return new List<AABB>();

			List<AABB> colliders = new();
			foreach (Entity entity in collidingEntities) {
				colliders.Add(entity.boundingBox);
			}
			return colliders;
		}
	}

	public static class EntityViewDefaults {
		public static List<Entity> GetEntities(this IEntityView view, Entity? except, AABB box) {
			return view.GetEntities(except, box, e => true);
		}

		public static List<AABB> GetEntityCollisions(this IEntityView view, AABB testBox) {
			return view.GetEntityCollisions(null, testBox);
		}
	}
}
