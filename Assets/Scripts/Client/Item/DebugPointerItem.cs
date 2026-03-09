using SoulboundBackend.Common;
using SoulboundBackend.Core.AssetManagement;
using SoulboundBackend.Core.Debug.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.ItemSystem {
	[PROTOTYPICAL]
	public sealed class DebugPointerItem : Item, IItemAction {
		public override string name => "Debug Pointer";
		public override int fullStackSize => 1;
		public override ItemAspect aspect => ItemAspect.Simple(new AssetKey("debugPointer"));

		public DebugPointerItem() : base("debugPointer") {
		}

		public bool ValidateTrigger(ItemActionTrigger trigger) {
			return trigger == ItemActionTrigger.LeftClick;
		}

		public bool CanExecute(ItemStack itemStack, ItemActionContext ctx) {
			return true;
		}

		public bool TryExecute(ItemStack itemStack, ItemActionContext ctx) {
			Logger.LogInfo("Pointer: {}", ctx.player.GetWorldPointerPos());
			return true;
		}
	}
}
