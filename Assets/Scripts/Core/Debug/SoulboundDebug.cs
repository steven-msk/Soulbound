using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Core.Debug {
	public class SoulboundDebug {
		public static SoulboundDebug instance { get; private set; }
		private readonly SoulboundLogHandler logHandler;

		public SoulboundDebug(ILogHandler unityLogHandler) {
			instance = this;
			this.logHandler = new SoulboundLogHandler(unityLogHandler);
		}

		public SoulboundLogHandler GetLogHandler() => logHandler;
	}
}
