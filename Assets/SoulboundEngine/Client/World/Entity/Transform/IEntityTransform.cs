using UnityEngine;

namespace SoulboundEngine.Client.World.EntitySystem.Transform {
	public interface IEntityTransform {

		// TODO: generalize entity transform fields such as physics and collisions
		// currently the transform leaves the implementation hidden for physics transforms
		// this encapsulation doesnt match the default way of entities to express their state
		// so PlayerTransform, PhysicsTransform and StaticTransform are obsolete because of this
		// with this being changed, ItemEntity will be able to encapsulate ItemEntityTransform logic.
		// this separation will become important for headless simulations later on

		void Bind(Entity entity);
		Entity GetEntity();

		Vector2 GetPos();
		void SetPos(Vector2 position);

		void FrameUpdate();

		void Destroy();
	}
}
