using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client {
	public interface IPlayerExecutionService {
		IInventoryExecutionService Inventory { get; }
		void SetPos(Vector2 pos);
	}
}
