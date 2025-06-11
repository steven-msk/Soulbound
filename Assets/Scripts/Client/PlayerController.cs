using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {

	[SerializeField] private InputHandler inputHandler;
	public InputHandler InputHandler => inputHandler;

	[SerializeField] private InventoryController inventory;
	public InventoryController Inventory => inventory;

	[SerializeField] private PlayerStats stats = new();
	public PlayerStats Stats => stats; 

	private PlayerPhysics playerPhysics;
	public PlayerPhysics Physics => playerPhysics;

	[Header("Combat")]
	[Obsolete] public float attackCooldown = 0.3f;
	[Obsolete] public int attackCombo = 0;
	[Obsolete] public int subAttackCombo = 0;

	[Header("Internal")]
	[SerializeField] private Rigidbody2D rb;
	public Rigidbody2D Rigidbody => rb;
	[SerializeField] private Animator animator;
	public Animator Animator => animator;

	[Header("Debug Internal")]
	[Obsolete] [SerializeField] private float knockbackStunTimer = 0f;
	[Obsolete] [SerializeField] private float attackTimer;

	private ItemUsageHandler itemUsageHandler;
	public ItemStack MainHandItem { get; private set; }

	private void Start() {
		playerPhysics = gameObject.GetComponent<PlayerPhysics>();

		itemUsageHandler = new ItemUsageHandler(this);
		itemUsageHandler.Register<IConsumable>(ItemUseTrigger.LeftClick, (item, stack) => item.Consume(stack, this));
		itemUsageHandler.Register<IAttackPerformer>(ItemUseTrigger.LeftClick, (item, stack) => item.PerformAttack(this));

		// alt attack?
		itemUsageHandler.Register<IAttackPerformer>(ItemUseTrigger.LeftHold, (item, stack) => Debug.Log(stack));
	}

	private void Update() {
		animator.SetFloat("horizontalSpeed", knockbackStunTimer <= 0 ? Mathf.Abs(rb.linearVelocityX) : 0);
		if (attackTimer > 0) {
			attackTimer -= Time.deltaTime;
		}

		if ((inputHandler.LeftHold || inputHandler.RightHold) && attackTimer <= 0) {
			Vector2 mousePos = inputHandler.MouseScreenPosition;
			transform.localScale = new Vector3(mousePos.x >= Screen.width / 2 ? 1 : -1, 1, 1);

			if (inputHandler.LeftHold) {
				itemUsageHandler.HandleInput(ItemUseTrigger.LeftHold);
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

	public void EquipHotbarItem([AllowsNull] ItemStack itemStack) {
		MainHandItem = itemStack;
	}

	public void OnLeftClick(InputAction.CallbackContext actionContext) {
		itemUsageHandler.HandleInput(ItemUseTrigger.LeftClick);
	}

	public void OnRightClick(InputAction.CallbackContext actionContext) {
		itemUsageHandler.HandleInput(ItemUseTrigger.RightClick);
	}

	public void OnSpacePressed(InputAction.CallbackContext actionContext) {
		playerPhysics.OnSpacePressed();
	}
}