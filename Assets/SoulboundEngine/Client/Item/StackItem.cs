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
	public sealed class StackItem : Item {
		public override string name => $"Stack Item: {fullStackSize}";

		public override ItemAspect aspect => ItemAspect.Simple(new AssetKey("idkwhatthisis"));

		public override int fullStackSize { get; }

		public StackItem(int fullStackSize)
			: base($"stackItem_{fullStackSize}") {
			this.fullStackSize = fullStackSize;
		}
	}
}
