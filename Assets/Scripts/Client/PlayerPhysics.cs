using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System.Linq;
using UnityEditor.ShaderGraph.Internal;
using System.Collections;

public class PlayerPhysics : MonoBehaviour {
#pragma warning disable CS8632
	private readonly Dictionary<string, (Action<Collision2D> action, Func<bool>? validator)> collisionReactionsByTag = new();
	private PlayerController player;
	private PlayerStats stats;
	private InputHandler inputHandler;
	private Rigidbody2D rb;
	private Animator animator;
	private BoxCollider2D boxCollider;

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

	private void Awake() {
		LogUtil.LogAwake(this);
	}

	// FIXME: inconsistent movement
	// When the player holds either left or right mouse button while moving towards the opposite
	// direction, the player is thrown towards the opposite direction (the facing where the mouse
	// button is held).

	private void Start() {
		player = GameManager.instance.Player;
		inputHandler = player.InputHandler;
		rb = player.Rigidbody;
		animator = player.Animator;
		stats = player.Stats;
		boxCollider = this.GetComponent<BoxCollider2D>();

		collisionReactionsByTag.Add("Enemy", ((collision) => {
			Vector2 bounce = (transform.position - (Vector3)collision.GetContact(0).point).normalized;
			rb.linearVelocity = bounce * contactBouncePower;
			knockbackStunTimer = knockbackStunDuration;
		}, null));

		collisionReactionsByTag.Add("Ground", ((collision) => {
			animator.SetBool("jumping", false);
			animator.SetBool("flying", false);
			animator.SetBool("onGround", true);
			flightTime = stats.GrantedFlightTime;
			jumpsLeft = stats.MaxJumps;
			isFlying = false;
			rb.linearDamping = 0f;
		}, IsOnGround));

		Debug.Assert(inputHandler.isActiveAndEnabled, inputHandler);
	}

	private void Update() {
		movement.x = inputHandler.HorizontalMovement;
		if (movement.x != 0 && !(inputHandler.LeftHold || inputHandler.RightHold)) {
			transform.localScale = new Vector3(Mathf.Sign(movement.x), 1, 1);
		}
	}

	private void FixedUpdate() {
		if (knockbackStunTimer > 0) {
			knockbackStunTimer -= Time.fixedDeltaTime;
			return;         // knockback immunity will be a thing
		}

		if (movement.x != 0) {
			if (!isFlying) {
				rb.linearVelocityX += stats.HorizontalAcceleration * movementSpeedPower * Time.fixedDeltaTime * movement.x;
				float speedLimit = stats.MovementSpeed.GetProcessedValue();
				if (Mathf.Abs(rb.linearVelocityX) > speedLimit) {
					rb.linearVelocityX = player.Facing * speedLimit;
				}
			} else {
				float scaledFlightAcceleration = flightMovementPower * stats.HorizontalFlightAcceleration;
				rb.linearVelocityX += scaledFlightAcceleration * Time.fixedDeltaTime * movement.x;
				if (rb.linearVelocityX > scaledFlightAcceleration) {
					rb.linearVelocityX = scaledFlightAcceleration;
				}
			}
		}
		if (shouldJump) {
			rb.AddForceY(stats.JumpHeight.GetProcessedValue() * jumpHeightPower, ForceMode2D.Impulse);
			shouldJump = false;
			animator.SetBool("onGround", false);
			rb.linearDamping = 1f;
		}

		if (jumpToFlightTimer > 0) {
			jumpToFlightTimer -= Time.fixedDeltaTime;
			return;         // flight switch will occur once jump timer is finished
		}
		if (inputHandler.PressingSpace && jumpsLeft == 0 && !shouldJump && flightTime > 0 && jumpToFlightTimer <= 0) {
			float scaledFlightAcceleration = flightMovementPower * stats.VerticalFlightAcceleration;
			rb.linearVelocityY += scaledFlightAcceleration * Time.fixedDeltaTime;
			if (rb.linearVelocityY > scaledFlightAcceleration) {
				rb.linearVelocityY = scaledFlightAcceleration;
			}
			isFlying = true;
			animator.SetBool("flying", true);
			animator.SetBool("jumping", false);
			rb.linearDamping = 1;
			flightTime -= Time.fixedDeltaTime * flightTimeReductionMultiplier;
			flightTime = Mathf.Clamp(flightTime, 0, stats.GrantedFlightTime);
		} else if (flightTime == 0 && inputHandler.PressingSpace && stats.GrantedFlightTime > 0) {
			if (rb.linearVelocityY <= -rb.gravityScale) {
				float scaledSlowFallVelocity = Time.fixedDeltaTime * slowFallTimeReductionMultiplier;
				rb.linearVelocityY = Mathf.Lerp(rb.linearVelocityY, -scaledSlowFallVelocity, scaledSlowFallVelocity);
			}
		}
		UpdateFlightTimePanel(isFlying, flightTime, stats.GrantedFlightTime);
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
		var collisionResponse = collisionReactionsByTag.GetValueOrDefault(collision.gameObject.tag, ((collision) => {
			Debug.Log($"Unknown collision callback tag: {collision.gameObject.tag}");
		}, null));
		collisionResponse.action.InvokeIf(collision, collisionResponse.validator);
	}

	public bool IsOnGround() {
		Vector2 origin = (Vector2)transform.position + boxCollider.offset + Vector2.down * (boxCollider.size.y * 0.5f);
		float offsetX = boxCollider.size.x * 0.5f;
		float distance = 0.1f;
		int layerMask = LayerMask.GetMask("Ground");

		RaycastHit2D leftHit = Physics2D.Raycast(origin - new Vector2(offsetX, 0), Vector2.down, distance, layerMask);
		RaycastHit2D middleHit = Physics2D.Raycast(origin, Vector2.down, 0.1f, layerMask);
		RaycastHit2D rightHit = Physics2D.Raycast(origin + new Vector2(offsetX, 0), Vector2.down, 0.1f, layerMask);
		return middleHit.collider != null || leftHit.collider != null || rightHit.collider != null;
	}

	public void UpdateFlightTimePanel(bool isFlying, float flightTime, float grantedFlightTime) {
		flightTimePanel.SetActive(isFlying && inputHandler.PressingSpace);
		if (flightTimePanel.activeSelf) {
			RectMask2D timeMask = flightTimePanel.GetComponentInChildren(typeof(RectMask2D), true) as RectMask2D;
			timeMask.padding = new Vector4(maskWidth * (1 - flightTime / grantedFlightTime), 0, 0, 0);
		}
	}
}