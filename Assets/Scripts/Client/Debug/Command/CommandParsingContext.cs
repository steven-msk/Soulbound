using SoulboundBackend.Client;
using SoulboundBackend.Client.Runtime.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.Debug.Commands {
	public sealed record CommandParsingContext(
		CommandArguments Args,
		IRuntimeDataProvider Data,
		IRuntimeExecutionServices ExecServices
	);
}
