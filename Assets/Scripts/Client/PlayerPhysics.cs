using SoulboundBackend.Client;
using SoulboundBackend.Client.Input;
using SoulboundBackend.Core;
using Logger = SoulboundBackend.Common.Logging.Logger;
using System;
using System.Collections.Generic;
using UnityEngine;
using SoulboundBackend.Common;
using UnityEngine.UI;
using SoulboundBackend.Core.Bootstrap;
using Zenject;
using System.Runtime.Serialization;

#nullable enable

namespace SoulboundBackend.Client {
	public class PlayerPhysics : MonoBehaviour {
		private static readonly Logger logger = Logger.CreateInstance();
		private readonly Dictionary<string, (Action<Collision2D> action, Func<bool> validator)> collisionReactionsByTag = new();
		private readonly Dictionary<int, (Action<Collider2D> action, Func<bool> validator)> triggerReactionsByLayer = new();
		private PlayerController player = null!;
		private InputHandler inputHandler = null!;
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
		[SerializeField] private Vector2 movement;
		[SerializeField] private float knockbackStunTimer = 0f;
		[SerializeField] private bool shouldJump = false;
		[SerializeField] private float jumpToFlightTimer;

		private float immunityTimeSeconds = 0.5f;

		public bool isBootstrapped { get; private set; }

		private float _facing = 1f;
		public float facing {
			get => _facing;
			set {
				_facing = value;
				Vector3 scale = transform.localScale;
				scale.x = _facing;
				transform.localScale = scale;
			}
		}

		[Inject]
		public void Construct(PlayerController player, InputHandler inputHandler) {
			this.player = player;
			this.inputHandler = inputHandler;
			this.rb = player.GetComponent<Rigidbody2D>();
			this.animator = player.GetComponent<Animator>();
			this.collider = player.GetComponent<CapsuleCollider2D>();


			//collisionReactionsByTag.Add("Enemy", ((collision) => {
			//	Vector2 bounce = (transform.position - (Vector3)collision.GetContact(0).point).normalized;
			//	rb.linearVelocity = bounce * contactBouncePower;
			//	knockbackStunTimer = knockbackStunDuration;
			//}, null));
			collisionReactionsByTag.Add("Ground", ((collision) => {
				animator.SetBool("jumping", false);
				animator.SetBool("flying", false);
				animator.SetBool("onGround", true);
				flightTime = player.Stats.GrantedFlightTime;
				jumpsLeft = player.Stats.MaxJumps;
				isFlying = false;
				rb.linearDamping = 0f;
			}, IsOnGround));

			triggerReactionsByLayer.Add(LayerMask.NameToLayer("Hitbox"), (collision => {
				player.TakeDamage(1);
				player.GrantImmunity(immunityTimeSeconds);
				Vector2 bounce = (transform.position - collision.transform.position).normalized;
				rb.linearVelocity = bounce * contactBouncePower;
				knockbackStunTimer = knockbackStunDuration * Mathf.Clamp(Mathf.Abs(bounce.y), 0.1f, 1f);
			}, () => !player.isImmune));
			isBootstrapped = true;
			UnityEngine.Debug.Log("player physics constructed");
		}

		// FIXME: inconsistent movement

		private void Update() {
			if (!player?.isSpawned ?? true) {
				return;
			}
			movement.x = inputHandler.HorizontalMovement;
			if (movement.x != 0) {
				this.facing = Mathf.Sign(movement.x);
			}
			if (inputHandler.RightHold || inputHandler.LeftHold) {
				this.facing = Mathf.Sign(inputHandler.MouseScreenPosition.x - Screen.width / 2);
			}
		}

		private void FixedUpdate() {
			if (!player?.isSpawned ?? true) {
				return;
			}
			if (knockbackStunTimer > 0) {
				knockbackStunTimer -= Time.fixedDeltaTime;
				return;         // knockback immunity will be a thing
			}

			if (movement.x != 0) {
				if (!isFlying) {
					rb.linearVelocityX += player.Stats.HorizontalAcceleration * movementSpeedPower * Time.fixedDeltaTime * movement.x;
					float speedLimit = player.Stats.MovementSpeed.GetProcessedValue();
					if (Mathf.Abs(rb.linearVelocityX) > speedLimit) {
						rb.linearVelocityX = Mathf.Sign(rb.linearVelocityX) * speedLimit;
					}
				} else {
					float scaledFlightAcceleration = flightMovementPower * player.Stats.HorizontalFlightAcceleration;
					rb.linearVelocityX += scaledFlightAcceleration * Time.fixedDeltaTime * movement.x;
					if (rb.linearVelocityX > scaledFlightAcceleration) {
						rb.linearVelocityX = scaledFlightAcceleration;
					}
				}
			} else {
				float deceleration = 10f;
				rb.linearVelocityX = Mathf.Lerp(rb.linearVelocityX, 0, deceleration * Time.fixedDeltaTime);
			}
			if (shouldJump) {
				rb.AddForceY(player.Stats.JumpHeight.GetProcessedValue() * jumpHeightPower, ForceMode2D.Impulse);
				shouldJump = false;
				animator.SetBool("onGround", false);
				rb.linearDamping = 1f;
			}

			if (jumpToFlightTimer > 0) {
				jumpToFlightTimer -= Time.fixedDeltaTime;
				return;         // flight switch will occur once jump timer is finished
			}
			if (inputHandler.PressingSpace && jumpsLeft == 0 && !shouldJump && flightTime > 0 && jumpToFlightTimer <= 0) {
				float scaledFlightAcceleration = flightMovementPower * player.Stats.VerticalFlightAcceleration;
				rb.linearVelocityY += scaledFlightAcceleration * Time.fixedDeltaTime;
				if (rb.linearVelocityY > scaledFlightAcceleration) {
					rb.linearVelocityY = scaledFlightAcceleration;
				}
				isFlying = true;
				animator.SetBool("flying", true);
				animator.SetBool("jumping", false);
				rb.linearDamping = 1;
				flightTime -= Time.fixedDeltaTime * flightTimeReductionMultiplier;
				flightTime = Mathf.Clamp(flightTime, 0, player.Stats.GrantedFlightTime);
			} else if (flightTime == 0 && inputHandler.PressingSpace && player.Stats.GrantedFlightTime > 0) {
				if (rb.linearVelocityY <= -rb.gravityScale) {
					float scaledSlowFallVelocity = Time.fixedDeltaTime * slowFallTimeReductionMultiplier;
					rb.linearVelocityY = Mathf.Lerp(rb.linearVelocityY, -scaledSlowFallVelocity, scaledSlowFallVelocity);
				}
			}
			UpdateFlightTimePanel(isFlying, flightTime, player.Stats.GrantedFlightTime);
		}

		internal void OnSpacePressed() { 
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
			if (!player?.isSpawned ?? true) {
				return;
			}
			var collisionResponse = collisionReactionsByTag.GetValueOrDefault(collision.gameObject.tag, (collision => {
				logger.LogWarning(null, "Unknown collision callback tag: {}", collision.gameObject.tag);
			}, null));
			collisionResponse.action.InvokeIf(collision, collisionResponse.validator ?? (() => true));
		}

		private void OnTriggerStay2D(Collider2D collision) {
			if (!player?.isSpawned ?? true) {
				return;
			}
			var triggerResponse = triggerReactionsByLayer.GetValueOrDefault(collision.gameObject.layer, (collision => {
				logger.LogWarning(null, "Unknown trigger callback layer: {}", LayerMask.LayerToName(collision.gameObject.layer));
			}, null));
			triggerResponse.action.InvokeIf(collision, triggerResponse.validator ?? (() => true));
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
			flightTimePanel.SetActive(isFlying && inputHandler.PressingSpace);
			if (flightTimePanel.activeSelf) {
				RectMask2D? timeMask = flightTimePanel.GetComponentInChildren(typeof(RectMask2D), true) as RectMask2D;
				timeMask!.padding = new Vector4(maskWidth * (1 - flightTime / grantedFlightTime), 0, 0, 0);
			}
		}
	}

}