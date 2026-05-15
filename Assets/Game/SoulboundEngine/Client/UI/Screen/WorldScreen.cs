using SoulboundEngine.Client.Input;
using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.ItemSystem.Container;
using SoulboundEngine.Client.Players;
using SoulboundEngine.Client.Render.Item;
using SoulboundEngine.Core.Assets;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.UI.Screen {
	public sealed class WorldScreen : UxmlScreen, IItemContainerScope, IInputEventHandler {
		private readonly Player player;
		private readonly ItemRenderManager itemRenderManager;
		private PlayerInventoryHandle inventoryHandle;
		private readonly List<IItemContainer> openContainers = new();
		private SlotDragState dragState;
		private TransitStack transitStack;
		private Vector2 pointerPosition;

		public WorldScreen(ItemRenderManager itemRenderManager, Player player) 
			: base(AssetManager.Resolve<VisualTreeAsset>(new AssetKey("WorldScreen"))) {
			this.itemRenderManager = itemRenderManager;
			this.player = player;
		}

		public override bool ReturnWithEscape => false;

		protected override void OnBind(VisualElement root) {
			this.inventoryHandle = new PlayerInventoryHandle(this.player.GetInventory(), this.itemRenderManager, this);
			this.inventoryHandle.OnBind(root.Q<VisualElement>("Inventory"));

			this.transitStack = new TransitStack(this.itemRenderManager, root.Q<VisualElement>("TransitStack"));
			this.player.SetTransitStackSource(this);

			// inventory is already opened when scene is loaded
			// TODO: add back inventory popup toggle functionality
			this.openContainers.Add(this.player.GetInventory());
		}

		public override void OnDispose(IScreenHandle handle) {
			this.inventoryHandle.Dispose();
		}

		public bool TryBeginDrag(ItemStack stack, SlotRef slotRef, int button) {
			if (this.InDragState() || stack == null) return false;

			HashSet<SlotRef> draggedSlots = new(new SlotRef.EqualityComparer()) { slotRef };

			this.dragState = new SlotDragState(slotRef.container) {
				stack = stack.Clone(),
				origin = slotRef,
				draggedSlots = draggedSlots,
				button = button,
				quantitySnapshots = this.CreateQuantitySnapshots(),
			};
			return true;
		}

		private Dictionary<SlotRef, int> CreateQuantitySnapshots() {
			Dictionary<SlotRef, int> snapshots = new();

			foreach (var container in this.openContainers) {
				Dictionary<int, int> quantities = this.GetQuantitySnapshotForContainer(container);

				foreach (var kvp in quantities) {
					SlotRef slotRef = new(container, kvp.Key);
					snapshots[slotRef] = kvp.Value;
				}
			}
			return snapshots;
		}

		private Dictionary<int, int> GetQuantitySnapshotForContainer(IItemContainer container) {
			return container.GetAllSlots()
					.Where(i => container.GetSlot(i).GetStack()?.quantity > 0)
					.ToDictionary(i => i, i => container.GetSlot(i).GetStack()!.quantity);
		}

		public void EndDrag() => this.dragState = null;

		public void ExtendDrag(SlotRef slotRef) {
			this.dragState?.ExtendDrag(slotRef);
		}

		public bool InDragState() => this.dragState != null;

		public SlotDragState GetDragState() => this.dragState;

		public IEnumerable<IItemContainer> GetOpenContainers() => this.openContainers;

		ItemStack ITransitStackSource.GetTransitStack() => this.transitStack.GetStack();

		bool ITransitStackSource.HasTransitStack() => this.transitStack.HasStack();
		
		void ITransitStackSource.SetTransitStack(ItemStack itemStack) {
			if (itemStack == null) this.transitStack.Destroy();
			else this.transitStack.SetStack(itemStack);
		}

		IEnumerable<InputEventListener> IInputEventHandler.GetListeners() {
			yield return InputEventListener.ObserveAny(InputTokens.Mouse.position, inputEvent => {
				this.pointerPosition = inputEvent.context.ReadValue<Vector2>();
				Vector2 converted = new(this.pointerPosition.x, UnityEngine.Device.Screen.height - this.pointerPosition.y);
				this.transitStack.SetPointerPosition(converted);
			});
		}

		public override void OnHide(IScreenHandle handle) {
			SoulboundClient.Instance.InputManager.RemoveHandler(this);
		}

		public override void OnShow(IScreenHandle handle) {
			SoulboundClient.Instance.InputManager.AddHandler(this);
		}
	}
}
