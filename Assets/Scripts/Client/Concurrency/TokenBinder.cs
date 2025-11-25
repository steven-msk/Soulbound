using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.Concurrency {
	public class TokenBinder : ActionBinder {
		public TokenBinder() : base(new ActionRequestData()) {
		}

		public ActionBinder UnderToken(ActionToken token) {
			actionRequestData.token = token;
			return this;
		}
	}
}
