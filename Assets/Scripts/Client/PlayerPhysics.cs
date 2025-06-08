using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPhysics : MonoBehaviour {

	private PlayerController player;
	private PlayerStats stats;
	private InputHandler inputHandler;
	private readonly Dictionary<string, Action<Collision2D>> collisionReactionsByTag = new();
	private Rigidbody2D rb;
	private Animator animator;

	public float movementSpeedPower = 30f;
	public float knockbackStunDuration = 0.5f;
	public float deceleration = 60f;
	public float contactBouncePower = 10f;
	public float jumpHeightPower = 45f;
	public float flightMovementPower = 20f;
	public float jumpToFlightDelay = 0.2f;

	[Header("Flight Time Panel")]								 // might me moved to a separate PlayerFlight script
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

	private void Start() {
		player = GameManager.GetPlayerInstance();
		inputHandler = player.InputHandler;
		rb = player.Rigidbody;
		animator = player.Animator;
		stats = player.Stats;

		collisionReactionsByTag.Add("Enemy", (collision) => {
			Vector2 bounce = (transform.position - (Vector3)collision.GetContact(0).point).normalized;
			rb.linearVelocity = bounce * contactBouncePower;
			knockbackStunTimer = knockbackStunDuration;
		});

		collisionReactionsByTag.Add("Ground", (collision) => {
			animator.SetBool("jumping", false);
			animator.SetBool("flying", false);
			animator.SetBool("onGround", true);
			flightTime = stats.grantedFlightTime;
			jumpsLeft = stats.maxJumps;
			isFlying = false;
			rb.linearDamping = 0f;
		});
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
				rb.linearVelocityX += stats.horizontalAcceleration * movementSpeedPower * Time.fixedDeltaTime * movement.x;
				if (Mathf.Abs(rb.linearVelocityX) > stats.movementSpeed) {
					rb.linearVelocityX = Mathf.Sign(rb.linearVelocityX) * stats.movementSpeed;

				}
			} else {
				float scaledFlightAcceleration = flightMovementPower * stats.horizontalFlightAcceleration;
				rb.linearVelocityX += scaledFlightAcceleration * Time.fixedDeltaTime * movement.x;
				if (rb.linearVelocityX > scaledFlightAcceleration) {
					rb.linearVelocityX = scaledFlightAcceleration;
				}
			}
		}
		if (shouldJump) {
			rb.AddForceY(stats.jumpHeight * jumpHeightPower, ForceMode2D.Impulse);
			shouldJump = false;
			animator.SetBool("onGround", false);
			rb.linearDamping = 1f;
		}

		if (jumpToFlightTimer > 0) {
			jumpToFlightTimer -= Time.fixedDeltaTime;
			return;         // flight switch will occur once jump is finished
		}
		if (inputHandler.PressingSpace && jumpsLeft == 0 && !shouldJump && flightTime > 0 && jumpToFlightTimer <= 0) {
			float scaledFlightAcceleration = flightMovementPower * stats.verticalFlightAcceleration;
			rb.linearVelocityY += scaledFlightAcceleration * Time.fixedDeltaTime;
			if (rb.linearVelocityY > scaledFlightAcceleration) {
				rb.linearVelocityY = scaledFlightAcceleration;
			}
			isFlying = true;
			animator.SetBool("flying", true);
			animator.SetBool("jumping", false);
			rb.linearDamping = 1;
			flightTime -= Time.fixedDeltaTime * flightTimeReductionMultiplier;
			flightTime = Mathf.Clamp(flightTime, 0, stats.grantedFlightTime);
		} else if (flightTime == 0 && inputHandler.PressingSpace && stats.grantedFlightTime > 0) {
			if (rb.linearVelocityY <= -rb.gravityScale) {
				float scaledSlowFallVelocity = Time.fixedDeltaTime * slowFallTimeReductionMultiplier;
				rb.linearVelocityY = Mathf.Lerp(rb.linearVelocityY, -scaledSlowFallVelocity, scaledSlowFallVelocity);
			}
		}
		UpdateFlightTimePanel(isFlying, flightTime, stats.grantedFlightTime);
	}

	public void OnSpacePressed() {
		if (jumpsLeft > 0 && knockbackStunTimer <= 0) {
			shouldJump = true;
			jumpsLeft--;
			jumpToFlightTimer = jumpToFlightDelay;
			if (!isFlying) {
				animator.SetBool("jumping", true);
			}
		}
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		collisionReactionsByTag.GetValueOrDefault(collision.gameObject.tag, (collision) => {
			Debug.Log($"Unknown collision callback tag: {collision.gameObject.tag}");
		}).Invoke(collision);
	}

	public void UpdateFlightTimePanel(bool isFlying, float flightTime, float grantedFlightTime) {
		flightTimePanel.SetActive(isFlying && inputHandler.PressingSpace);
		RectMask2D timeMask = flightTimePanel.GetComponentInChildren(typeof(RectMask2D), true) as RectMask2D;
		timeMask.padding = new Vector4(maskWidth * (1 - flightTime / grantedFlightTime), 0, 0, 0);
	}
}