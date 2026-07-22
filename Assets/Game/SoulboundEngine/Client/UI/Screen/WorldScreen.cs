using SoulboundEngine.Client.Debug;
using SoulboundEngine.Client.Debug.Logging.Console;
using SoulboundEngine.Client.Debug.Metrics.View;
using SoulboundEngine.Client.Item.Container;
using SoulboundEngine.Client.Player;
using SoulboundEngine.Client.Render.Item;
using SoulboundEngine.Core.Assets;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.UI.Screen {
	public sealed class WorldScreen : UxmlScreen {
		public const string COMMAND_LINE_ELEMENT = "CommandLine";
		public const string METRICS_HUD_ELEMENT = "MetricsHUD";
		public const string LOG_CONSOLE_ELEMENT = "LogConsole";
		public const string HOTBAR_ELEMENT = "Hotbar";
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

		public override bool CloseOnEsc => false;

		protected override void OnBind(VisualElement root) {
			this.commandLine.OnBind(root.Q<VisualElement>(COMMAND_LINE_ELEMENT));
			this.metricsHUD.OnBind(root.Q<VisualElement>(METRICS_HUD_ELEMENT));
			this.logConsole.OnBind(root.Q<VisualElement>(LOG_CONSOLE_ELEMENT));

			this.hotbarRoot = root.Q<VisualElement>(HOTBAR_ELEMENT);
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
