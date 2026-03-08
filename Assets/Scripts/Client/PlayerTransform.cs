using SoulboundBackend.Client.World.EntitySystem;
using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Logger = SoulboundBackend.Core.Debug.Logging.Logger;

#nullable enable

namespace SoulboundBackend.Client {
	public class PlayerTransform : MonoBehaviour, IEntityTransform {
		private readonly Dictionary<string, (Action<Collision2D> action, Func<bool> validator)> collisionReactionsByTag = new();
		private readonly Dictionary<int, (Action<Collider2D> action, Func<bool> validator)> triggerReactionsByLayer = new();
		private Player player = null!;
		private Rigidbody2D rb = null!;
		private Animator animator = null!;
		new private CapsuleCollider2D collider = null!;
		public CapsuleCollider2D Collider => collider;

		public float movementSpeedPower = 30f;
		public float knockbackStunDuration = 0.5f;
		public float deceleration = 60f;
		public float contactBouncePower = 10f;
		public float jumpHeightPower = 45f;
		public float flightMovementPower = 20f;
		public float jumpToFlightDelay = 0.2f;

		[Header("Flight Time Panel")]                                // might me moved to a separate PlayerFlight script
		[SerializeField] private GameObject flightTimePanel;
		[SerializeField] private float maskWidth = 50;               // maskWidth must be equal to TimeBar width (center&middle anchor)

		[Header("Internal")]
		public int jumpsLeft;
		[SerializeField] private float flightTime = 0;
		[SerializeField] private float flightTimeReductionMultiplier = 50;
		[SerializeField] private float slowFallTimeReductionMultiplier = 400;

		[Header("Debug Internal")]
		[SerializeField] private bool isFlying = false;
		[SerializeField] private Vector2 velocity;
		[SerializeField] private float knockbackStunTimer = 0f;
		[SerializeField] private bool shouldJump = false;
		[SerializeField] private float jumpToFlightTimer;
		[SerializeField] private float _facing;
		public float facing {
			get => _facing;
			set {
				_facing = value;
				Vector3 scale = transform.localScale;
				scale.x = _facing;
				transform.localScale = scale;
			}
		}

		void IEntityTransform.Bind(Entity entity) {
			this.player = (Player)entity;
			this.rb = GetComponent<Rigidbody2D>();
			this.animator = GetComponent<Animator>();
			this.collider = GetComponent<CapsuleCollider2D>();

			collisionReactionsByTag.Add("Ground", ((collision) => {
				animator.SetBool("jumping", false);
				animator.SetBool("flying", false);
				animator.SetBool("onGround", true);
				flightTime = player.Stats.grantedFlightTime;
				jumpsLeft = player.Stats.maxJumps;
				isFlying = false;
				rb.linearDamping = 0f;
			}, IsOnGround));
		}

		Entity IEntityTransform.GetEntity() => player;

		public Vector2 GetPos() => transform.position;
		public void SetPos(Vector2 position) => transform.position = position;

		void IEntityTransform.Destroy() => Destroy(gameObject);

		public void SetVelocityX(float x) => velocity.x = x;
		public void SetVelocityY(float y) => velocity.y = y;
		public void SetVelocity(Vector2 velocity) => this.velocity = velocity;
		public Vector2 GetVelocity() => velocity;


		// FIXME: inconsistent movement

		private void Update() {
			if (velocity.x != 0) {
				facing = Mathf.Sign(velocity.x);
			}
			if (player.IsHoldingRightClick() || player.IsHoldingLeftClick()) {
				facing = Mathf.Sign(player.GetScreenPointerPos().x - Screen.width / 2);
			}
			animator.SetFloat("horizontalSpeed", Mathf.Abs(velocity.x));
		}

		private void FixedUpdate() {
			if (knockbackStunTimer > 0) {
				knockbackStunTimer -= Time.fixedDeltaTime;
				return;         // knockback immunity will be a thing
			}

			if (velocity.x != 0) {
				if (!isFlying) {
					rb.linearVelocityX += player.Stats.horizontalAcceleration * movementSpeedPower * Time.fixedDeltaTime * velocity.x;
					float speedLimit = player.Stats.movementSpeed.GetProcessedValue();
					if (Mathf.Abs(rb.linearVelocityX) > speedLimit) {
						rb.linearVelocityX = Mathf.Sign(rb.linearVelocityX) * speedLimit;
					}
				} else {
					float scaledFlightAcceleration = flightMovementPower * player.Stats.horizontalFlightAcceleration;
					rb.linearVelocityX += scaledFlightAcceleration * Time.fixedDeltaTime * velocity.x;
					if (rb.linearVelocityX > scaledFlightAcceleration) {
						rb.linearVelocityX = scaledFlightAcceleration;
					}
				}
			} else {
				float deceleration = 10f;
				rb.linearVelocityX = Mathf.Lerp(rb.linearVelocityX, 0, deceleration * Time.fixedDeltaTime);
			}
			if (shouldJump) {
				rb.AddForceY(player.Stats.jumpHeight.GetProcessedValue() * jumpHeightPower, ForceMode2D.Impulse);
				shouldJump = false;
				animator.SetBool("onGround", false);
				rb.linearDamping = 1f;
			}

			if (jumpToFlightTimer > 0) {
				jumpToFlightTimer -= Time.fixedDeltaTime;
				return;         // flight switch will occur once jump timer is finished
			}
			if (jumpsLeft == 0 && !shouldJump && flightTime > 0 && jumpToFlightTimer <= 0) {
				float scaledFlightAcceleration = flightMovementPower * player.Stats.verticalFlightAcceleration;
				rb.linearVelocityY += scaledFlightAcceleration * Time.fixedDeltaTime;
				if (rb.linearVelocityY > scaledFlightAcceleration) {
					rb.linearVelocityY = scaledFlightAcceleration;
				}
				isFlying = true;
				animator.SetBool("flying", true);
				animator.SetBool("jumping", false);
				rb.linearDamping = 1;
				flightTime -= Time.fixedDeltaTime * flightTimeReductionMultiplier;
				flightTime = Mathf.Clamp(flightTime, 0, player.Stats.grantedFlightTime);
			} else if (flightTime == 0 && player.Stats.grantedFlightTime > 0) {
				if (rb.linearVelocityY <= -rb.gravityScale) {
					float scaledSlowFallVelocity = Time.fixedDeltaTime * slowFallTimeReductionMultiplier;
					rb.linearVelocityY = Mathf.Lerp(rb.linearVelocityY, -scaledSlowFallVelocity, scaledSlowFallVelocity);
				}
			}
			UpdateFlightTimePanel(isFlying, flightTime, player.Stats.grantedFlightTime);
		}

		internal void Jump() { 
			if (jumpsLeft > 0 && knockbackStunTimer <= 0) {
				shouldJump = true;
				jumpsLeft--;
				jumpToFlightTimer = jumpToFlightDelay;
				if (!isFlying) {
					animator.SetBool("jumping", true);
				}
			}
		}

		private void OnCollisionStay2D(Collision2D collision) {
			var collisionResponse = collisionReactionsByTag.GetValueOrDefault(collision.gameObject.tag, (collision => {
				Logger.LogWarning("Unknown collision callback tag: {}", collision.gameObject.tag);
			}, null));
			collisionResponse.action.InvokeIf(collision, collisionResponse.validator ?? (() => true));
		}
		public bool IsOnGround() {
			Vector2 origin = (Vector2)transform.position + collider.offset + Vector2.down * (collider.size.y * 0.5f);
			float offsetX = collider.size.x * 0.5f;
			float distance = 0.1f;
			int layerMask = LayerMask.GetMask("Ground");

			RaycastHit2D leftHit = Physics2D.Raycast(origin - new Vector2(offsetX, 0), Vector2.down, distance, layerMask);
			RaycastHit2D middleHit = Physics2D.Raycast(origin, Vector2.down, distance, layerMask);
			RaycastHit2D rightHit = Physics2D.Raycast(origin + new Vector2(offsetX, 0), Vector2.down, distance, layerMask);
			return middleHit.collider != null || leftHit.collider != null || rightHit.collider != null;
		}

		public void UpdateFlightTimePanel(bool isFlying, float flightTime, float grantedFlightTime) {
			flightTimePanel.SetActive(isFlying);
			if (flightTimePanel.activeSelf) {
				RectMask2D? timeMask = flightTimePanel.GetComponentInChildren(typeof(RectMask2D), true) as RectMask2D;
				timeMask!.padding = new Vector4(maskWidth * (1 - flightTime / grantedFlightTime), 0, 0, 0);
			}
		}
	}

}
