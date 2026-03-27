using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.World.EntitySystem;
using SoulboundEngine.Client.World.EntitySystem.Transform;
using SoulboundEngine.Core;
using SoulboundEngine.Core.Event;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.Players {
	[RequireComponent(typeof(Rigidbody2D))]
	public class PlayerTransform : MonoBehaviour, IEntityTransform, IItemPickupHandler {
		private Player player = null!;
		private Rigidbody2D rb = null!;
		new private CapsuleCollider2D collider = null!;
		public CapsuleCollider2D Collider => collider;

		[Header("Debug info")]
		[SerializeField] private Vector2 normalVelocity;
		[SerializeField] private Vector2 rbVelocity;
		[SerializeField] private bool isGrounded;
		[SerializeField] private bool isJumping;

		[Header("Data knobs")]
		[SerializeField] private float speed = 1f;
		[SerializeField] private float jumpForce = 1f;

		void IEntityTransform.Bind(Entity entity) {
			this.player = (Player)entity;
			this.rb = GetComponent<Rigidbody2D>();
			this.collider = GetComponent<CapsuleCollider2D>();
		}

		Entity IEntityTransform.GetEntity() => player;
		public Player GetEntity() => player;

		public Vector2 GetPos() => rb.position;
		public void SetPos(Vector2 position) => rb.position = position;

		void IEntityTransform.Destroy() => Destroy(gameObject);

		private void Update() {
			rb.linearVelocity = new Vector2(normalVelocity.x * speed, rb.linearVelocityY);
			rbVelocity = rb.linearVelocity;
		}

		private void FixedUpdate() {
			UpdateIsGrounded();
			if (isJumping && isGrounded) Jump();
		}

		internal void SetJumping(bool value) {
			this.isJumping = value;
		}

		public void Jump() {
			rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
			// PROTOTYPICAL
			this.isGrounded = false;
			EventBus.Publish(new PlayerJumpedEvent(player));
		}

		private void UpdateIsGrounded() {
			int groundMask = LayerMask.GetMask(Layers.Ground);
			RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.05f, groundMask);
			UnityEngine.Debug.DrawRay(transform.position, Vector2.down * 1.05f, Color.red, 1, false);
			isGrounded = hit.collider != null;
		}

		public void SetNormalVelocityX(float x) => normalVelocity.x = x;
		public void SetNormalVelocityY(float y) => normalVelocity.y = y;
		public void SetNormalVelocity(Vector2 velocity) => normalVelocity = velocity;
		public Vector2 GetNormalVelocity() => normalVelocity;

		bool IItemPickupHandler.TryPickupStack(ItemStack itemStack) {
			return player.TryAddItemStack(itemStack);
		}

	}
}
