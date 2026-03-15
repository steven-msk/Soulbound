using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundBackend.Client.ItemSystem {
	public interface ITransitStackSource {
		ItemStack? GetTransitStack();
	}
}
