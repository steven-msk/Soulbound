using SoulboundEngine.Client.Debug;
using SoulboundEngine.Client.Debug.Logging.Console;
using SoulboundEngine.Client.Debug.Metrics.View;
using SoulboundEngine.Client.Input;
using SoulboundEngine.Client.ItemSystem.Container;
using SoulboundEngine.Client.Render.Item;
using SoulboundEngine.Core.Assets;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.UI.Screen {
	using Player = Player.Player;

	public sealed class WorldScreen : UxmlScreen, IInputEventHandler {
		int IInputEventHandler.priority => 5005;
		private readonly Player player;
		private readonly CommandLine commandLine;
		private readonly MetricsHUD metricsHUD;
		private readonly LogConsole logConsole;
		private readonly ItemRenderManager itemRenderManager;
		private PlayerInventoryHandle inventoryHandle; 
		private TransitStack transitStack;
		private Vector2 pointerPosition;

		public WorldScreen(ItemRenderManager itemRenderManager, Player player, CommandLine commandLine, MetricsHUD metricsHUD, LogConsole logConsole) 
			: base(AssetManager.Resolve<VisualTreeAsset>(new AssetKey("WorldScreen"))) {
			this.itemRenderManager = itemRenderManager;
			this.player = player;
			this.commandLine = commandLine;
			this.metricsHUD = metricsHUD;
			this.logConsole = logConsole;
		}

		public override bool EscapeReturn => false;

		protected override void OnBind(VisualElement root) {
			this.BindInventory(root);
			this.BindDebug(root);
		}

		private void BindInventory(VisualElement root) {
			this.inventoryHandle = new PlayerInventoryHandle(this.player.GetInventory(), this.itemRenderManager, this.player);
			this.inventoryHandle.OnBind(root.Q<VisualElement>("Inventory"));

			this.transitStack = new TransitStack(this.itemRenderManager, root.Q<VisualElement>("TransitStack"));
			this.player.SetTransitStackSource(this.transitStack);

			// inventory is open at init
			this.inventoryHandle.Close();
		}

		private void BindDebug(VisualElement root) {
			this.commandLine.OnBind(root.Q<VisualElement>("CommandLine"));
			this.metricsHUD.OnBind(root.Q<VisualElement>("MetricsHUD"));
			this.logConsole.OnBind(root.Q<VisualElement>("LogConsole"));
		}

		public override void OnDispose(IScreenHandle handle) {
			this.inventoryHandle.Dispose();
			this.commandLine.Dispose();
		}

		IEnumerable<InputEventListener> IInputEventHandler.GetListeners() {
			yield return InputEventListener.ConsumePerformed(InputTokens.Player.toggleInventory, _ => {
				if (!this.inventoryHandle.isOpen) {
					this.inventoryHandle.Open();
					this.player.OpenInventory();
				} else { 
					this.inventoryHandle.Close();
					this.player.CloseInventory();
				}
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
