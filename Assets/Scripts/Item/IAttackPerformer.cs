using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// special subject to major changes
public interface IAttackPerformer : IItemCapability {
	void PerformAttack(PlayerController player);
}
