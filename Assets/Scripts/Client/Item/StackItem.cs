using SoulboundBackend.Common;
using SoulboundBackend.Core.AssetManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.ItemSystem {
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
