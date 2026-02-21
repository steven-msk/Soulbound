using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Core.Debug {
	public class SoulboundDebug {

		public SoulboundDebug(ILogger logger) {
			new Logging.Logger(logger);
		}
	}
}
