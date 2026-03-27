using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.Runtime.Services {
	public interface IRuntimeDataProvider {
		IRuntimePlayerDataProvider Player { get; }
		IRuntimeEntityDataProvider Entities { get; }
	}
}
