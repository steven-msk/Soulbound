using SoulboundBackend.Client;
using SoulboundBackend.Client.Input;
using SoulboundBackend.Core;
using Logger = SoulboundBackend.Core.Debug.Logging.Logger;
using System;
using System.Collections.Generic;
using UnityEngine;
using SoulboundBackend.Common;
using UnityEngine.UI;
using SoulboundBackend.Core.Bootstrap;
using Zenject;
using System.Runtime.Serialization;
using static PlayerInputActions;

#nullable enable

namespace SoulboundBackend.Client {
	public class PlayerPhysics : MonoBehaviour {
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

		private bool leftHold;
		private bool rightHold;
		private Vector2 mouseScreenPos;

		[Inject]
		public void Construct(DiContainer container) {
			this.player = container.Resolve<PlayerController>();
			this.inputHandler = container.Resolve<InputHandler>();
			this.rb = player.GetComponent<Rigidbody2D>();
			this.animator = player.GetComponent<Animator>();
			this.collider = player.GetComponent<CapsuleCollider2D>();

			collisionReactionsByTag.Add("Ground", ((collision) => {
				animator.SetBool("jumping", false);
				animator.SetBool("flying", false);
				animator.SetBool("onGround", true);
				flightTime = player.Stats.grantedFlightTime;
				jumpsLeft = player.Stats.maxJumps;
				isFlying = false;
				rb.linearDamping = 0f;
			}, IsOnGround));

			inputHandler.RegisterInputEvent(inputHandler.GetAction("Move"), pausable: true, binding => {
				binding.Performed(context => movement.x = context.ReadValue<Vector2>().x);
				binding.Canceled(context => movement.x = 0f);
			});
			inputHandler.RegisterInputEvent(inputHandler.GetAction("LeftClick"), pausable: true, binding => {
				binding.Performed(_ => leftHold = true);
				binding.Performed(_ => leftHold = false);
			});
			inputHandler.RegisterInputEvent(inputHandler.GetAction("RightClick"), pausable: true, binding => {
				binding.Performed(_ => rightHold = true);
				binding.Canceled(_ => rightHold = false);
			});
			inputHandler.RegisterInputEvent(inputHandler.GetAction("MousePosition"), pausable: false, (action) => {
				action.Performed(context => mouseScreenPos = context.ReadValue<Vector2>());
			});
			inputHandler.RegisterInputEvent(inputHandler.GetAction("Jump"), pausable: true, (action) => {
				action.Performed(_ => OnSpacePressed());
			});
		}

		// FIXME: inconsistent movement

		private void Update() {
			if (!player?.isSpawned ?? true) {
				return;
			}
			animator.SetFloat("horizontalSpeed", Mathf.Abs(movement.x));
			if (movement.x != 0) {
				this.facing = Mathf.Sign(movement.x);
			}
			if (rightHold || leftHold) {
				this.facing = Mathf.Sign(mouseScreenPos.x - Screen.width / 2);
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
					rb.linearVelocityX += player.Stats.horizontalAcceleration * movementSpeedPower * Time.fixedDeltaTime * movement.x;
					float speedLimit = player.Stats.movementSpeed.GetProcessedValue();
					if (Mathf.Abs(rb.linearVelocityX) > speedLimit) {
						rb.linearVelocityX = Mathf.Sign(rb.linearVelocityX) * speedLimit;
					}
				} else {
					float scaledFlightAcceleration = flightMovementPower * player.Stats.horizontalFlightAcceleration;
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
