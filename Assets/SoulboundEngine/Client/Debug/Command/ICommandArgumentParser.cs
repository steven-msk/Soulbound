using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.Debug.Commands {
	public interface ICommandArgumentParser<T> {
		ParseResult<T> TryParse(string token, CommandParsingContext ctx);
	}
}
