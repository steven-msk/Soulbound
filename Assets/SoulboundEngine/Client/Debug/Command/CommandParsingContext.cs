using SoulboundEngine.Client;
using SoulboundEngine.Client.Runtime.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.Debug.Commands {
	public sealed record CommandParsingContext(
		CommandArguments Args,
		IRuntimeDataProvider Data,
		IRuntimeExecutionServices ExecServices
	);
}
