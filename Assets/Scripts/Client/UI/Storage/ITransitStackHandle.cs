using SoulboundBackend.Client.ItemSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.UI {
	public interface ITransitStackHandle {
		void Init(ItemDisplay display);
		ItemStack GetStack();
		void Destroy();
	}
}
