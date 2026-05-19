using SoulboundEngine.Client.Debug;
using SoulboundEngine.Client.Debug.Logging.Console;
using SoulboundEngine.Client.Debug.Metrics.View;
using SoulboundEngine.Client.ItemSystem.Container;
using SoulboundEngine.Client.Player;
using SoulboundEngine.Client.Render.Item;
using SoulboundEngine.Core.Assets;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.UI.Screen {
	public sealed class WorldScreen : UxmlScreen {
		private readonly ItemRenderManager itemRenderManager;
		private readonly CommandLine commandLine;
		private readonly MetricsHUD metricsHUD;
		private readonly LogConsole logConsole;
		private readonly PlayerInventory playerInventory;
		private HotbarSlotDisplay[] hotbarDisplays;
		private VisualElement hotbarRoot;

		public WorldScreen(PlayerInventory playerInventory, CommandLine commandLine, MetricsHUD metricsHUD, LogConsole logConsole, ItemRenderManager itemRenderManager)
			: base(AssetManager.Resolve<VisualTreeAsset>(new AssetKey("WorldScreen"))) {
			this.commandLine = commandLine;
			this.metricsHUD = metricsHUD;
			this.logConsole = logConsole;
			this.playerInventory = playerInventory;
			this.itemRenderManager = itemRenderManager;
		}

		public override bool EscapeReturn => false;

		protected override void OnBind(VisualElement root) {
			this.commandLine.OnBind(root.Q<VisualElement>("CommandLine"));
			this.metricsHUD.OnBind(root.Q<VisualElement>("MetricsHUD"));
			this.logConsole.OnBind(root.Q<VisualElement>("LogConsole"));

			this.hotbarRoot = root.Q<VisualElement>("Hotbar");
			this.BindHotbar(this.hotbarRoot);
		}

		private void BindHotbar(VisualElement hotbarRoot) {
			this.hotbarDisplays = new HotbarSlotDisplay[PlayerInventory.HOTBAR_SIZE];

			foreach (var slotIndex in this.playerInventory.GetHotbar()) {
				IItemSlot slot = this.playerInventory.GetSlot(slotIndex);
				VisualElement slotElement = hotbarRoot[slotIndex];

				HotbarSlotDisplay display = new(slot, this.itemRenderManager);
				display.OnBind(slotElement);
				this.hotbarDisplays[slotIndex] = display;
			}

			this.playerInventory.mainSlotChanged += this.OnMainSlotChanged;
			this.SetAsMainSlotVisual(this.playerInventory.GetMainSlot());
		}

		public void SetHotbarVisible(bool visible) {
			if (this.hotbarRoot != null) {
				this.hotbarRoot.visible = visible;
			}
		}

		private void OnMainSlotChanged(int oldIndex, int newIndex) {
			this.UnsetMainSlotVisual(oldIndex);
			this.SetAsMainSlotVisual(newIndex);
		}

		private void SetAsMainSlotVisual(int slot) {
			this.hotbarDisplays[slot].SetAsMainSlot();
		}

		private void UnsetMainSlotVisual(int slot) {
			this.hotbarDisplays[slot].UnsetMainSlot();
		}

		public override void OnDispose(IScreenHandle handle) {
			this.commandLine.Dispose();

			for (int i = 0; i < this.hotbarDisplays.Length; i++) {
				this.hotbarDisplays[i].Dispose();
			}
		}
	}
}
