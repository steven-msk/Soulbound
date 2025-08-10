using System;
using System.Collections;
using System.Collections.Generic;
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

#nullable enable

	private ItemUsageHandler itemUsageHandler;
	public ItemUsageHandler ItemUsageHandler => itemUsageHandler!;
	public ItemStack? MainHandStack { get; private set; }

	public bool CanAttack { get; set; } = true;

	private int lastFacing = 1;
	public float facing {
		get {
			if (Rigidbody.linearVelocityX != 0f) {
				lastFacing = (int)Mathf.Sign(Rigidbody.linearVelocityX);
			}
			return lastFacing;
		}
	}
	public Vector2 position => transform.position;
	public Vector2 center => Physics.Collider.bounds.center;
	public BlockPos blockPos => level.ToBlockPos(this.position);
	public ChunkBlockPos chunkBlockPos => blockPos.ToChunkBlockPos(level.ChunkXAt(position));

    // FIXME: inconsistency in item drop force direction
    public Vector2 itemDropForce {
		get {
			Vector2 force = new Vector2(3f, 4f);
			force.x *= facing;
			return force;
        }
	}

	public float MaxBlockReach => 5f;

	private void Awake() {
		playerPhysics = gameObject.GetComponent<PlayerPhysics>();
		itemUsageHandler = new ItemUsageHandler(this);
		level = GameManager.instance.Level;
		itemUsageHandler.Register<IConsumable>(ItemUseTrigger.RightClick, (consumable, stack) => consumable.Consume(stack));
		foreach (ItemUseTrigger trigger in Enum.GetValues(typeof(ItemUseTrigger))) {
			itemUsageHandler.Register<IAttackPerformer>(trigger, (attackPerformer, stack) => {
				CanAttack.If(() => attackPerformer.PerformAttack(trigger));
			});
		}
        itemUsageHandler.Register<IPlaceable>(ItemUseTrigger.LeftHold, (Action<IPlaceable, ItemStack>)((placeable, stack) => {
            Level level = GameManager.instance.Level;
            BlockPos blockPos = level.ToBlockPos(inputHandler.MouseWorldPosition);

			if (this.IsInBlockReach((Vector2Int)blockPos) && level.BlockAt(blockPos) == Blocks.air) {
				level.SetBlock(blockPos, placeable.Place(stack, blockPos));
			}
		}));
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
		static void InvokeStatItem(ItemStack? itemStack, Action<IStatProvider> statProviderAction) {
			if (itemStack?.Item is IStatProvider statProvider && statProvider.applyInstantStatsAutomatically) {
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
	internal void OnLeftClick() => RequestSuppressedMainHandUse(ItemUseTrigger.LeftClick);

	[InputAction("ItemUse", Priority = 5)]
	internal void OnRightClick() => RequestMainHandUse(ItemUseTrigger.RightClick, null);

	// POTENTIAL FEATUREIMPL: add Reach int stat

    [InputAction("ItemUse", Priority = 5)]
	internal void OnLeftHold() { 
		if (MainHandStack != null) {
			RequestSuppressedMainHandUse(ItemUseTrigger.LeftHold);
        } else {
			InputHandler.RequestAction(new InputActionRequest("BlockBreak", 5, () => {
				Level level = GameManager.instance.Level;
				Vector2 worldMousePos = inputHandler.MouseWorldPosition;
				BlockPos blockPos = level.ToBlockPos(worldMousePos);

				if (this.IsInBlockReach(worldMousePos) && level.BlockAt(blockPos) != Blocks.air) {
					level.BreakBlock(blockPos, BreakSource.Player);
                }
			}, null));
		}
	}

	[InputAction("ItemUse", Priority = 5)]
	internal void OnRightHold() => RequestMainHandUse(ItemUseTrigger.RightHold, null);

	private void RequestSuppressedMainHandUse(ItemUseTrigger trigger) {
        Item? usedItem = MainHandStack?.Item;
		RequestMainHandUse(trigger, null);
        InputHandler.RequestAction(new("ItemUse", 5, () => itemUsageHandler.HandleInput(trigger, MainHandStack), null));
        if (usedItem is IPlaceable) {
            InputHandler.BlockContext("BlockBreak", () => !InputHandler.LeftHold);
        }
    }

	private void RequestMainHandUse(ItemUseTrigger trigger, Action? callback) {
		InputHandler.RequestAction(new("ItemUse", 5, () => itemUsageHandler.HandleInput(trigger, MainHandStack), callback));
	}

	public bool IsInBlockReach(Vector2 worldPos) {
		float dist = Vector2.Distance(worldPos, this.center);
		return dist <= MaxBlockReach && dist > 1.5f;
	}
}