using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {

	[SerializeField] private InputHandler inputHandler;
	public InputHandler InputHandler { get => inputHandler; }
	[SerializeField] private InventoryController inventory;
	public InventoryController Inventory { get => inventory; }
	[SerializeField] private PlayerStats stats;

	[Header("Movement")]
	public float movementSpeedPower = 20f;
	public float knockbackStunDuration = 0.5f;
	public float deceleration = 60f;
	public float contactBouncePower = 20f;
	public float jumpHeightPower = 1300f;
	public float flightMovementPower = 10f;
	public float jumpToFlightDelay = 0.2f;

	[Header("Combat")]
	public float attackCooldown = 0.3f;
	public int attackCombo = 0;
	public int subAttackCombo = 0;

	[Header("Internal")]
	public Rigidbody2D rb;
	public Animator animator;
	public int jumpsLeft;
	[SerializeField] private float flightTime;
	[SerializeField] private float flightTimeReductionMultiplier;
	[SerializeField] private float slowFallTimeReductionMultiplier;

	[Header("Debug Internal")]
	[SerializeField] private bool isFlying = false;
	[SerializeField] private Vector2 movement;
	[SerializeField] private float knockbackStunTimer = 0f;
	[SerializeField] private float attackTimer;
	[SerializeField] private bool shouldJump = false;
	[SerializeField] private float jumpToFlightTimer;
	private readonly Dictionary<string, Action<Collision2D>> collisionReactionsByTag = new();

	private ItemUsageHandler itemUsageHandler;
	public ItemStack MainHandItem { get; private set; }

	private void Start() {
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

		itemUsageHandler = new ItemUsageHandler(this);
		itemUsageHandler.Register<ConsumableItem>(ItemUseTrigger.LeftClick, (item, stack) => item.Consume(stack, this));
	}

	private void Update() {
		movement.x = inputHandler.HorizontalMovement;
		if (movement.x != 0 && !(inputHandler.LeftHold || inputHandler.RightHold)) {
			transform.localScale = new Vector3(Mathf.Sign(movement.x), 1, 1);
		}

		animator.SetFloat("horizontalSpeed", knockbackStunTimer <= 0 ? Mathf.Abs(rb.linearVelocityX) : 0);
		if (attackTimer > 0) {
			attackTimer -= Time.deltaTime;
		}

		if ((inputHandler.LeftHold || inputHandler.RightHold) && attackTimer <= 0) {
			Vector2 mousePos = inputHandler.MouseScreenPosition;
			transform.localScale = new Vector3(mousePos.x >= Screen.width / 2 ? 1 : -1, 1, 1);

			if (inputHandler.LeftHold) {

			} else if (inputHandler.RightHold) {

				// [deprecated]
				GameObject CreateBeam(float yoffset, Vector3 pos, Quaternion rotation, Vector2 facing) {
					GameObject beam = GameObject.Instantiate(animator.GetComponent<AttackHitbox>().beam, pos, rotation);
					beam.GetComponent<BeamController>().facing = facing;
					beam.SetActive(true);
					return beam;
				}
				animator.SetBool("attacking", true);
				animator.SetTrigger("attack");
				
				Vector3 mouseWorldPos = inputHandler.MouseWorldPosition;
				Vector3 facing = (mouseWorldPos - transform.position).normalized;
				Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(facing.y, facing.x) * Mathf.Rad2Deg - 90f);

				if (attackCombo >= 2) {
					Vector2 perp = new(-facing.y, facing.x);
					Vector3 convergePos = transform.position + facing * (subAttackCombo % 2 == 0 ? 5f : 2f);

					for (float offset = -1f ; offset < 1f; offset += 0.2f) {
						Vector3 pos = transform.position + (Vector3)(perp * offset);
						Vector3 subFacing = (convergePos - pos).normalized;
						CreateBeam(offset, pos, rotation, subFacing);
					}
					
					attackCombo = 0;
					subAttackCombo++;
				}
				CreateBeam(0, transform.position, rotation, facing);
				attackCombo++;
				attackTimer = attackCooldown;
			}
		}
	}

	private void FixedUpdate() {
		if (knockbackStunTimer > 0) {
			knockbackStunTimer -= Time.fixedDeltaTime;
			return;			// knockback immunity will be a thing
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
			return;			// flight switch will occur once jump is finished
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
		GameManager.GetUI().UpdateFlightTimePanel(isFlying, flightTime, stats.grantedFlightTime);
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		collisionReactionsByTag.GetValueOrDefault(collision.gameObject.tag, (collision) => {
			Debug.Log($"Unknown collision callback tag: {collision.gameObject.tag}");
		}).Invoke(collision);
	}

	public void EquipHotbarItem([AllowsNull] ItemStack itemStack) {
		MainHandItem = itemStack;
	}

	public void OnLeftClick(InputAction.CallbackContext actionContext) {
		if (EventPriorityManager.IsAllowed("ItemUse")) {
			itemUsageHandler.HandleInput(ItemUseTrigger.LeftClick);
		}
	}

	public void OnRightClick(InputAction.CallbackContext actionContext) {
	}

	public void OnSpacePressed(InputAction.CallbackContext actionContext) {
		if (jumpsLeft > 0 && knockbackStunTimer <= 0) {
			shouldJump = true;
			jumpsLeft--;
			jumpToFlightTimer = jumpToFlightDelay;
			if (!isFlying) { 
				animator.SetBool("jumping", true);
			}
		}
	}
}