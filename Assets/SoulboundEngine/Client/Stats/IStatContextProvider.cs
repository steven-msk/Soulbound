using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.Stats {
	public interface IStatContextProvider {
		IStatModificationHost statModificationHost { get; }
	}
}
