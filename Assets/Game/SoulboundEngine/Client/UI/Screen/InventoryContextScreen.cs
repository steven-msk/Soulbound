using SoulboundEngine.Client.Input;
using SoulboundEngine.Client.ItemSystem.Container;
using SoulboundEngine.Client.Render.Item;
using SoulboundEngine.Core.Assets;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.UI.Screen {
	using Player = Player.Player;

	public class InventoryContextScreen : UxmlScreen, IInputEventHandler {
		int IInputEventHandler.priority => 5005;
		private readonly ItemRenderManager itemRenderManager;
		private readonly Player player;
		private PlayerInventoryHandle inventoryHandle;
		private TransitStack transitStack;
		private Vector2 pointerPosition;

		public InventoryContextScreen(ItemRenderManager itemRenderManager, Player player) 
			: base(AssetManager.Resolve<VisualTreeAsset>(new AssetKey("InventoryContextScreen"))) {
			this.itemRenderManager = itemRenderManager;
			this.player = player;
		}

		public override bool IsOpaque => false;

		protected override void OnBind(VisualElement root) {
			this.inventoryHandle = new PlayerInventoryHandle(this.player.GetInventory(), this.itemRenderManager, this.player);
			this.inventoryHandle.OnBind(root.Q<VisualElement>("PlayerInventorySpace"));

			this.transitStack = new TransitStack(this.itemRenderManager, root.Q<VisualElement>("TransitStack"));
			this.player.SetTransitStackSource(this.transitStack);
		}

		IEnumerable<InputEventListener> IInputEventHandler.GetListeners() {
			yield return InputEventListener.ConsumePerformed(InputTokens.Player.toggleInventory, _ => {
				//if (!this.inventoryHandle.isOpen) {
				//	this.inventoryHandle.Open();
				//	this.player.OpenInventory();
				//} else {
				//	this.inventoryHandle.Close();
				//	this.player.CloseInventory();
				//}
			});

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
