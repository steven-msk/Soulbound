using SoulboundBackend.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core.Debug.Commands {
	public sealed record CommandParsingContext(CommandArguments Args, IDynamicDataProvider Data);
}
