
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.World.Services {
	public interface IRuntimeExecutionServices {
		IPlayerExecutionService Player { get; }
		IEntityExecutionService Entity { get; }
		ILevelExecutionService Level { get; }
	}
}
