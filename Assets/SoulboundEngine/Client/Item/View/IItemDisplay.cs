using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.ItemSystem.View {
	public interface IItemDisplay {
		void SetStack(ItemStack itemStack);
		ItemStack GetStack();

		void Destroy();
		bool IsDestroyed();
	}
}
