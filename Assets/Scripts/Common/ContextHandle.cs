using SoulboundBackend.Client.Stats;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundBackend.Common {
	public class ContextHandle<T> {
		public bool hasContext { get; set; }
		public event Action<T>? onContextReceived;
		public event Action<T>? onContextLost;

		public void OnContextReceived(T context) {
			onContextReceived?.Invoke(context);
			hasContext = true;
		}

		public void OnContextLost(T context) {
			if (this.hasContext) {
				onContextLost?.Invoke(context);
			}
			hasContext = false;
		}
	}
}
