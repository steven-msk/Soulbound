using System;
using System.Collections;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour {
	[SerializeField] private InputHandler inputHandler;
	public InputHandler InputHandler => inputHandler;

	[SerializeField] private InventoryController inventory;
	public InventoryController Inventory => inventory;

	[SerializeField] private PlayerStats stats = new();
	public PlayerStats Stats => stats; 

	private PlayerPhysics playerPhysics;
	public PlayerPhysics Physics => playerPhysics;

	private Level level;

	[Header("Internal")]
	[SerializeField] private Rigidbody2D rb;
	public Rigidbody2D Rigidbody => rb;
	[SerializeField] private Animator animator;
	public Animator Animator => animator;

	private ItemUsageHandler itemUsageHandler;
	public ItemUsageHandler ItemUsageHandler => itemUsageHandler;
	public ItemStack MainHandStack { get; private set; }

	public bool CanAttack { get; set; } = true;

	public float facing => Mathf.Sign(transform.localScale.x);
	public Vector2 position => transform.position;
	public BlockPos blockPos => level.ToBlockPos(this.position);
	public ChunkBlockPos chunkBlockPos => blockPos.ToChunkBlockPos(level.ChunkXAt(position));

	private void Awake() {
		playerPhysics = gameObject.GetComponent<PlayerPhysics>();
		itemUsageHandler = new ItemUsageHandler(this);
		level = GameManager.instance.Level;
		itemUsageHandler.Register<IConsumable>(ItemUseTrigger.RightClick, (consumable, stack) => consumable.Consume(stack));
		foreach (ItemUseTrigger trigger in Enum.GetValues(typeof(ItemUseTrigger))) {
			itemUsageHandler.Register<IAttackPerformer>(trigger, (attackPerformer, stack) => {
				InvocationHelper.If(CanAttack, () => attackPerformer.PerformAttack(trigger));
			});
		}
		itemUsageHandler.Register<IPlaceable>(ItemUseTrigger.LeftHold, (placeable, stack) => {
			Level level = GameManager.instance.Level;
			BlockPos blockPos = level.ToBlockPos(inputHandler.MouseWorldPosition);

			if (level.TileAt(blockPos) == CommonTiles.air) {
				level.SetBlockAndUpdate(blockPos, placeable.Place(stack, blockPos));
			}
		});
		LogUtil.LogAwake(this);
	}

	private void Start() {
		transform.SetPositionAndRotation(new(position.x, level.GetSurfaceY(blockPos.x), transform.position.z), Quaternion.identity);
	}

	private void Update() {
		animator.SetFloat("horizontalSpeed", Mathf.Abs(rb.linearVelocityX));

		if (inputHandler.LeftHold || inputHandler.RightHold) {
			Vector2 mousePos = inputHandler.MouseScreenPosition;
			transform.localScale = new Vector3(mousePos.x >= Screen.width / 2 ? 1 : -1, 1, 1);
		}

		GameManager.instance.Level.UpdateChunks(transform.position);
	}

	public void SetMainHandItem([AllowsNull] ItemStack itemStack) {
		if (MainHandStack == itemStack) {
			return;
		}
		static void InvokeStatItem(ItemStack itemStack, Action<IStatProvider> statProviderAction) {
			if (itemStack?.Item is IStatProvider statProvider && statProvider.ApplyInstantStatsAutomatically) {
				statProviderAction.Invoke(statProvider);
			}
		}
		InvokeStatItem(MainHandStack, statProvider => statProvider.RevokeInstantStats(this.stats));
		InvokeStatItem(MainHandStack, statProvider => statProvider.DisableBuffers(this.stats));
		MainHandStack = itemStack;
		InvokeStatItem(MainHandStack, statProvider => statProvider.ApplyInstantStats(this.stats));
		InvokeStatItem(MainHandStack, statProvider => statProvider.EnableBuffers(this.stats));
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