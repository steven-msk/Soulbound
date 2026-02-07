using SoulboundBackend.Client.UI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.UI {
	public sealed class WorldScreen : Screen {
		private readonly PlayerController player;

		public WorldScreen(PlayerController player) {
			this.player = player;
		}

		protected override void OnBuild(IScreenObject screenObject) {
			IPlayerInventoryHandle playerInventory = new PlayerInventoryUIBuilder(player.GetInventory()).Build(screenObject);
		}
	}
}
