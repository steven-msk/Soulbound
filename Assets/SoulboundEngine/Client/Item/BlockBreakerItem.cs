using SoulboundEngine.Client.ItemSystem.View;
using SoulboundEngine.Common;
using SoulboundEngine.Core.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.ItemSystem {
	[PROTOTYPICAL]
	public sealed class BlockBreakerItem : Item, IBlockBreakerItem {
		public override string name => $"Block Breaker Item";
		public override ItemAspect aspect => ItemAspect.Simple(new AssetKey("bluething"));

		public BlockBreakerItem() : base("blockBreakerItem") {
		}

		public int GetBreakLevel(ItemStack itemStack) => 1;
	}
}
