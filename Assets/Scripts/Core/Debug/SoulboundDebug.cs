using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Core.Debug {
	public class SoulboundDebug {
		public static SoulboundDebug instance { get; private set; }
		private readonly ILogger logger;

		public SoulboundDebug(ILogger logger) {
			instance = this;
			this.logger = logger;
			new Logging.Logger(logger);
		}

		public ILogger GetLogger() => logger;
	}
}
