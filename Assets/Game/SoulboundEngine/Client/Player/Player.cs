using SoulboundEngine.Client.Input;
using SoulboundEngine.Client.Interaction;
using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.ItemSystem.Container;
using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Client.World.EntitySystem;
using SoulboundEngine.Client.World.EntitySystem.Transform;
using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Common;
using SoulboundEngine.Core.Assets;
using SoulboundEngine.Core.Event;
using UnityEngine;
using UnityEngine.InputSystem;

#nullable enable

namespace SoulboundEngine.Client.Players {
	public class Player : Entity, IInputContext, IInteractionHandler<ItemInteraction>, IInteractionHandler<BlockInteraction> {
		public static readonly EntityDescriptor<Player> DESCRIPTOR = EntityDescriptor.Of(
			(_, level) => new Player(level),
			ITransformSupplier<Player>.Of(entity => {
				GameObject obj = GameObject.Instantiate(AssetManager.Resolve<GameObject>(new AssetKey("player")));
				PlayerTransform transform = obj.GetComponent<PlayerTransform>();
				return transform;
			})
		);
		private readonly Inventory inventory;
		private readonly Hotbar hotbar;
		private ITransitStackSource tranistStackSource = null!;
		private PlayerTransform playerTransform = null!;
		private new Vector2 initialPos;

		// TODO: interaction resolver shouldnt be created by the player
		private readonly InteractionResolver interactionResolver = new();

		const float MAX_BLOCK_REACH = 5f;

		private Vector2 screenPointerPos;
		private bool isHoldingLeftClick;
		private bool isHoldingRightClick;
		private bool isHoldingCtrl;

		// provisory guard for not breaking the block instantly after it was placed
		private bool leftClickBlockBreakGuard;

		public Player(Level level)
			: base(DESCRIPTOR, level) {
			this.inventory = new Inventory();
			this.hotbar = new Hotbar();

			this.interactionResolver.RegisterHandler<ItemInteraction>(this);
			this.interactionResolver.RegisterHandler<BlockInteraction>(this);
		}

		protected override void OnTransformCreated(IEntityTransform transform) {
			this.playerTransform = (PlayerTransform)transform;
		}

		[PROTOTYPICAL]
		bool IInputContext.HandleInput(in InputEvent inputEvent) {
			if (inputEvent.token.Equals(InputTokens.Player.toggleInventory)) {
				this.inventory.Toggle();
				return true;
			}
			if (inputEvent.Performed(InputTokens.Player.changeHotbarSlot)) {
				int slotIndex = int.Parse(inputEvent.context.control.name) - 1;
				this.hotbar.SetMainSlotIndex(slotIndex);
				return true;
			}
			if (inputEvent.Performed(InputTokens.Player.scrollHotbarSlot)) {
				float scrollDelta = inputEvent.context.ReadValue<float>();
				int nextSlot = this.hotbar.GetMainSlotIndex() - (int)scrollDelta;

				if (nextSlot < 0) nextSlot += this.hotbar.GetSlotCount();
				nextSlot %= this.hotbar.GetSlotCount();
				this.hotbar.SetMainSlotIndex(nextSlot);
				return true;
			}
			if (inputEvent.token.Equals(InputTokens.Mouse.position)) {
				this.screenPointerPos = inputEvent.context.ReadValue<Vector2>();
			}
			if (inputEvent.token.Equals(InputTokens.Mouse.leftClick)) {
				if (inputEvent.phase == InputActionPhase.Performed) {
					this.OnLeftClick();
					this.isHoldingLeftClick = true;
					return true;
				} else if (inputEvent.phase == InputActionPhase.Canceled) {
					this.OnLeftRelease();
					this.isHoldingLeftClick = false;
					return true;
				}
			}
			if (inputEvent.token.Equals(InputTokens.Mouse.rightClick)) {
				if (inputEvent.phase == InputActionPhase.Performed) {
					this.OnRightClick();
					this.isHoldingRightClick = true;
					return true;
				} else if (inputEvent.phase == InputActionPhase.Canceled) {
					this.OnRightRelease();
					this.isHoldingRightClick = false;
					return true;
				}
			}

			if (inputEvent.token.Equals(InputTokens.Player.move)) {
				if (inputEvent.phase == InputActionPhase.Performed) {
					this.playerTransform.SetNormalVelocityX(inputEvent.context.ReadValue<Vector2>().x);
					return true;
				} else if (inputEvent.phase == InputActionPhase.Canceled) {
					this.playerTransform.SetNormalVelocityX(0f);
					return true;
				}
			}
			if (inputEvent.token.Equals(InputTokens.Player.jump)) {
				if (inputEvent.phase == InputActionPhase.Performed) {
					this.playerTransform.SetJumping(true);
					return true;
				} else if (inputEvent.phase == InputActionPhase.Canceled) {
					this.playerTransform.SetJumping(false);
					return true;
				}
			}

			if (inputEvent.Performed(InputTokens.Keyboard.Q)) {
				this.ThrowFromMainHand(this.isHoldingCtrl);
				return true;
			}
			if (inputEvent.token.Equals(InputTokens.Keyboard.CTRL)) {
				if (inputEvent.phase == InputActionPhase.Performed) {
					this.isHoldingCtrl = true;
				} else if (inputEvent.phase == InputActionPhase.Canceled) {
					this.isHoldingCtrl = false;
				}
			}
			return false;
		}

		public override void FrameUpdate() {
			base.FrameUpdate();
			if (this.isHoldingLeftClick) this.OnLeftHold();
			if (this.isHoldingRightClick) this.OnRightHold();
		}

		private void OnLeftClick() {
			if (!this.ResolveItemOrBlockInteraction(InteractionTrigger.LeftClick)) {

				// PROTOTYPICAL
				BlockPos blockPos = (BlockPos)this.GetWorldPointerPos();
				if (this.TryBreakBlock(blockPos)) {
					EventBus.Publish(new BlockBrokenEvent(blockPos, this.level));
				}
			}
		}
		private void OnRightClick() {
			this.ResolveItemOrBlockInteraction(InteractionTrigger.RightClick);
		}

		private void OnLeftHold() {
			if (!this.ResolveItemOrBlockInteraction(InteractionTrigger.LeftHold)) {

				// PROTOTYPICAL
				BlockPos blockPos = (BlockPos)this.GetWorldPointerPos();
				if (this.TryBreakBlock(blockPos)) {
					EventBus.Publish(new BlockBrokenEvent(blockPos, this.level));
				}
			}
		}
		private void OnRightHold() {
			this.ResolveItemOrBlockInteraction(InteractionTrigger.RightHold);
		}

		private void OnLeftRelease() {
			this.ResolveItemOrBlockInteraction(InteractionTrigger.LeftRelease);
			this.leftClickBlockBreakGuard = false;
		}
		private void OnRightRelease() {
			this.ResolveItemOrBlockInteraction(InteractionTrigger.RightRelease);
		}

		private bool ResolveItemOrBlockInteraction(InteractionTrigger trigger) {
			ItemInteraction itemInteraction = this.GetItemInteraction(trigger);
			if (this.interactionResolver.Resolve(itemInteraction)) {
				this.leftClickBlockBreakGuard = itemInteraction.itemStack?.item is IPlaceableItem;
				return true;
			}
			return this.interactionResolver.Resolve(this.GetBlockInteraction(trigger));
		}

		private ItemInteraction GetItemInteraction(InteractionTrigger trigger) {
			return new ItemInteraction {
				itemStack = this.GetMainHandStack(),
				player = this,
				level = this.level,
				trigger = trigger
			};
		}

		private BlockInteraction GetBlockInteraction(InteractionTrigger trigger) {
			BlockPos blockPos = (BlockPos)this.GetWorldPointerPos();
			return new BlockInteraction {
				trigger = trigger,
				blockPos = blockPos,
				blockState = this.level.GetBlockState(blockPos),
				itemStack = this.GetMainHandStack(),
				level = this.level
			};
		}

		// provisory priority
		int IInteractionHandler<ItemInteraction>.priority => 0;

		bool IInteractionHandler<ItemInteraction>.CanHandle(in ItemInteraction ctx) {
			Item? item = ctx.itemStack?.item;
			if (item is not IItemInteractionListener listener) return false;

			if (!listener.ValidateTrigger(ctx.trigger)) return false;

			return listener.CanExecute(ctx.itemStack, in ctx);
		}

		bool IInteractionHandler<ItemInteraction>.Handle(in ItemInteraction ctx) {
			ItemStack stack = ctx.itemStack;
			IItemInteractionListener listener = (IItemInteractionListener)stack.item;
			return listener.TryExecute(stack, in ctx);
		}

		int IInteractionHandler<BlockInteraction>.priority => 0;

		bool IInteractionHandler<BlockInteraction>.CanHandle(in BlockInteraction ctx) {

			// interaction handler shouldnt guard block interactions only inside the player reach
			// some blocks may be interactable even if theyre out of reach, though this is a false assumption for pre-prod
			// CanInteract will need to explicitly check if the player is in range if it requires it
			// for this case the handler is implemented in Player so the this CanHandle guards it
			// but keep this in mind for future implementations
			bool isInReach = this.IsInBlockReach((Vector2)ctx.blockPos);
			if (!isInReach) return false;

			if (ctx.blockState.block is not IBlockInteractionListener listener) return false;

			if (!listener.ValidateTrigger(ctx.trigger)) return false;

			return listener.CanInteract(in ctx);
		}

		bool IInteractionHandler<BlockInteraction>.Handle(in BlockInteraction ctx) {
			IBlockInteractionListener listener = (IBlockInteractionListener)ctx.blockState.block;
			listener.OnInteract(in ctx);
			return true;
		}

		private bool TryBreakBlock(BlockPos blockPos) {
			if (!this.IsInBlockReach((Vector2)blockPos) || this.leftClickBlockBreakGuard) return false;

			BlockState blockState = this.level.GetBlockState(blockPos) ?? Blocks.air.DefaultState;
			if (blockState.block == Blocks.air) return false;

			int itemBreakLevel = this.GetMainHandItemBreakLevel();
			int minBreakLevel = blockState.block.minBreakLevel;
			if (itemBreakLevel < minBreakLevel) return false;

			this.level.SetBlockState(blockPos, Blocks.air.DefaultState);
			return true;
		}

		private int GetMainHandItemBreakLevel() {
			ItemStack? mainHandStack = this.GetMainHandStack();
			Item? item = mainHandStack?.item;

			if (item == null) return 0;

			if (item is IBlockBreakerItem breaker) {
				return breaker.GetBreakLevel(mainHandStack);
			}
			return -1;
		}

		private void ThrowFromMainHand(bool ctrl) {
			ItemStack? mainHandStack = this.GetMainHandStack();
			if (mainHandStack == null) return;

			int throwAmount = ctrl ? mainHandStack.quantity : 1;
			ItemStack thrownStack = mainHandStack.Clone(throwAmount);
			mainHandStack.Decrement(throwAmount);

			ItemEntity itemEntity = new(this, pickupDelaySec: 2f, thrownStack, this.level);
			this.level.AddEntity(itemEntity);
		}

		public bool TryAddItemStack(ItemStack itemStack) {
			if (this.inventory.TryAddStack(itemStack)) return true;
			return this.hotbar.TryAddStack(itemStack);
		}

		public bool CanPlaceBlockAt(BlockPos blockPos) {
			Vector2 worldPos = (Vector2)blockPos;
			return this.IsInBlockReach(worldPos)
				   && this.level?.GetBlock(blockPos) == Blocks.air;
		}

		public bool CanBreakBlockAt(BlockPos blockPos) {
			Vector2 worldPos = (Vector2)blockPos;
			return this.IsInBlockReach(worldPos)
				   && this.level?.GetBlock(blockPos) != Blocks.air;
		}

		public bool IsInBlockReach(Vector2 worldPos) {
			float dist = Vector2.Distance(worldPos, this.GetCenter());
			return dist <= MAX_BLOCK_REACH 
				&& !this.level.GetTilesCovered(this.playerTransform.Collider.bounds)
						 .Contains((BlockPos)worldPos);
		}

		public Inventory GetInventory() => this.inventory;
		public Hotbar GetHotbar() => this.hotbar;

		public ItemStack? GetMainHandStack() {
			ItemStack? transitStack = this.tranistStackSource?.GetTransitStack();
			return transitStack ?? this.hotbar.GetSlot(this.hotbar.GetMainSlotIndex()).GetStack();
		}

		public Vector2 GetCenter() => this.playerTransform.Collider.bounds.center;

		public bool IsHoldingLeftClick() => this.isHoldingLeftClick;
		public bool IsHoldingRightClick() => this.isHoldingRightClick;

		public Vector2 GetScreenPointerPos() => this.screenPointerPos;
		public Vector2 GetWorldPointerPos() {
			Vector3 screenPos = this.screenPointerPos;

			Canvas canvas = SoulboundClient.Instance.UIHandler.GetCanvas();
			RectTransform rootTransform = canvas.GetComponent<RectTransform>();
			bool inWorldPoint = RectTransformUtility.ScreenPointToWorldPointInRectangle(
				rootTransform,
				screenPos,
				Camera.main,
				out var worldPoint
			);
			if (inWorldPoint) return worldPoint;

			screenPos.z = -Camera.main.transform.position.z;
			return Camera.main.ScreenToWorldPoint(screenPos);
		}

		public override void SetPos(Vector2 pos) {
			if (this.transform != null) this.transform.SetPos(pos);
			else this.initialPos = pos;
		}
		public override Vector2 GetPos() => this.transform?.GetPos() ?? this.initialPos;

		public void SetTransitStackSource(ITransitStackSource source) {
			this.tranistStackSource = source;
		}
	}
}
