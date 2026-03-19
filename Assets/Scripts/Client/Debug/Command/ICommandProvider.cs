using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.Debug.Commands {
	public interface ICommandProvider {
		IEnumerable<CommandNode> GetCommands();
	}
}
