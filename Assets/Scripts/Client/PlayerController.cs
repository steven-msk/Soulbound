using System;
using System.Collections;
using Unity.IO.LowLevel.Unsafe;
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

	[Header("Internal")]
	[SerializeField] private Rigidbody2D rb;
	public Rigidbody2D Rigidbody => rb;
	[SerializeField] private Animator animator;
	public Animator Animator => animator;

	private ItemUsageHandler itemUsageHandler;
	public ItemUsageHandler ItemUsageHandler => itemUsageHandler;
	public ItemStack MainHandStack { get; private set; }

	public bool CanAttack { get; set; } = true;

	public float Facing => Mathf.Sign(transform.localScale.x);

	private void Start() {
		playerPhysics = gameObject.GetComponent<PlayerPhysics>();
		itemUsageHandler = new ItemUsageHandler(this);
		itemUsageHandler.Register<IConsumable>(ItemUseTrigger.LeftClick, (consumable, stack) => consumable.Consume(stack));
		foreach (ItemUseTrigger trigger in Enum.GetValues(typeof(ItemUseTrigger))) {
			itemUsageHandler.Register<IAttackPerformer>(trigger, (attackPerformer, stack) => {
				if (CanAttack) {
					attackPerformer.PerformAttack(trigger);
				}
			});
		}
	}

	private void Update() {
		animator.SetFloat("horizontalSpeed", Mathf.Abs(rb.linearVelocityX));

		if (inputHandler.LeftHold || inputHandler.RightHold) {
			Vector2 mousePos = inputHandler.MouseScreenPosition;
			transform.localScale = new Vector3(mousePos.x >= Screen.width / 2 ? 1 : -1, 1, 1);
		}
	}

	public void EquipHotbarItem([AllowsNull] ItemStack itemStack) {
		if (MainHandStack == itemStack) {
			return;
		}
		MainHandStack = itemStack;
		if (itemStack?.Item is IStatProvider statProvider && statProvider.ApplyStatsAutomatically) {
			statProvider.ApplyStats(this);
		}
	}

	[InputAction("ItemUse", Priority = 5)]
	internal void OnLeftClick() => InputHandler.RequestAction(ItemUseRequest(ItemUseTrigger.LeftClick));

	[InputAction("ItemUse", Priority = 5)]
	internal void OnRightClick() => InputHandler.RequestAction(ItemUseRequest(ItemUseTrigger.RightClick));

	[InputAction("ItemUse", Priority = 5)]
	internal void OnLeftHold() => InputHandler.RequestAction(ItemUseRequest(ItemUseTrigger.LeftHold));

	[InputAction("ItemUse", Priority = 5)]
	internal void OnRightHold() => InputHandler.RequestAction(ItemUseRequest(ItemUseTrigger.RightHold));

	private InputActionRequest ItemUseRequest(ItemUseTrigger useTrigger) => new InputActionRequest("ItemUse", 5, () => itemUsageHandler.HandleInput(useTrigger));
}