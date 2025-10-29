using SoulboundBackend.Client.Combat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.ItemSystem {
	public interface IAttackSourceProvider : IItemCapability {
		bool GetAttackSource(ItemUseTrigger trigger, out AttackSource source);
	}
}
