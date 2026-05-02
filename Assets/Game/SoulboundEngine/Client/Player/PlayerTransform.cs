using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.World.EntitySystem;
using SoulboundEngine.Core;
using SoulboundEngine.Core.Event;
using UnityEngine;
using IEntityView = SoulboundEngine.Client.Render.Entity.IEntityView;

#nullable enable

namespace SoulboundEngine.Client.Players {
	[RequireComponent(typeof(Rigidbody2D))]
	[RequireComponent(typeof(CapsuleCollider2D))]
	public class PlayerTransform : MonoBehaviour, IEntityView, Entity.IPhysicsHandle, Entity.IBoundingBoxHandle, Player.IPlayerTransformHandle, IItemPickupHandler {
		private Player player = null!;
		private Rigidbody2D rb = null!;
		new private CapsuleCollider2D collider = null!;
		public CapsuleCollider2D Collider => this.collider;

		[Header("Debug info")]
		[SerializeField] private Vector2 normalVelocity;
		[SerializeField] private Vector2 rbVelocity;
		[SerializeField] private bool isGrounded;
		[SerializeField] private bool isJumping;

		[Header("Data knobs")]
		[SerializeField] private float speed = 1f;
		[SerializeField] private float jumpForce = 1f;

		public Player GetEntity() => this.player;

		public void Init(Player player) {
			this.player = player;
			this.rb = this.GetComponent<Rigidbody2D>();
			this.collider = this.GetComponent<CapsuleCollider2D>();
		}

		public Vector2 GetPos() => this.rb.position;
		public void SetPos(Vector2 position) => this.rb.position = position;

		private void Update() {
			this.rb.linearVelocity = new Vector2(this.normalVelocity.x * this.speed, this.rb.linearVelocityY);
			this.rbVelocity = this.rb.linearVelocity;
		}

		private void FixedUpdate() {
			this.UpdateIsGrounded();
			if (this.isJumping && this.isGrounded) this.Jump();
		}

		public void SetJumping(bool value) {
			this.isJumping = value;
		}

		public void Jump() {
			this.rb.linearVelocity = new Vector2(this.rb.linearVelocityX, this.jumpForce);
			// PROTOTYPICAL
			this.isGrounded = false;
			EventBus.Publish(new PlayerJumpedEvent(this.player));
		}

		private void UpdateIsGrounded() {
			int groundMask = LayerMask.GetMask(Layers.Ground);
			RaycastHit2D hit = Physics2D.Raycast(this.transform.position, Vector2.down, 1.05f, groundMask);
			UnityEngine.Debug.DrawRay(this.transform.position, Vector2.down * 1.05f, Color.red, 1, false);
			this.isGrounded = hit.collider != null;
		}

		public void SetNormalVelocityX(float x) => this.normalVelocity.x = x;
		public void SetNormalVelocityY(float y) => this.normalVelocity.y = y;
		public void SetNormalVelocity(Vector2 velocity) => this.normalVelocity = velocity;
		public Vector2 GetNormalVelocity() => this.normalVelocity;

		bool IItemPickupHandler.TryPickupStack(ItemStack itemStack) {
			return this.player.TryAddItemStack(itemStack);
		}

		public GameObject GetGameObject() => this.gameObject;

		public void SetVisible(bool visible) => this.gameObject.SetActive(visible);

		public void Destroy() => GameObject.Destroy(this.gameObject);

		public void SetVelocity(Vector2 velocity) {
			this.rb.linearVelocity = velocity;
		}

		public Vector2 GetVelocity() => this.rb.linearVelocity;

		public Vector2 GetPosition() => this.rb.position;

		public void SetPosition(Vector2 pos) => this.rb.position = pos;

		public void ApplyForce(Vector2 force) => this.rb.AddForce(force);

		public Bounds GetBoundingBox() => this.collider.bounds;
	}
}
