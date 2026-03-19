
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.Runtime.Services {
	public interface IRuntimeExecutionServices {
		IPlayerExecutionService Player { get; }
		IEntityExecutionService Entity { get; }
		ILevelExecutionService Level { get; }
	}
}
