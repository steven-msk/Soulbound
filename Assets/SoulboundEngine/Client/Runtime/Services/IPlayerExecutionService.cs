using SoulboundEngine.Client.ItemSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundEngine.Client.Runtime.Services {
	public interface IPlayerExecutionService {
		IInventoryExecutionService Inventory { get; }
		void SetPos(Vector2 pos);
		bool TryAddItemStack(ItemStack itemStack);
	}
}
