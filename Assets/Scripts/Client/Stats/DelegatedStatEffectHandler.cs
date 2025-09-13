using System;
using System.Collections.Generic;

namespace SoulboundBackend.Client.Stats {
	public sealed class DelegatedStatEffectHandler : IStatEffectHandler {
		private IEnumerable<AbstractSerializableStat> suppliedStats;
		private Action<IStatReceiver> enableAction;
		private Action<IStatReceiver> disableAction;

		public DelegatedStatEffectHandler(Action<IStatReceiver> enableAction, Action<IStatReceiver> disableAction, IEnumerable<AbstractSerializableStat> suppliedStats) {
			this.enableAction = enableAction;
			this.disableAction = disableAction;
			this.suppliedStats = suppliedStats;
		}

		public void Enable(IStatReceiver receiver) {
			enableAction.Invoke(receiver);
		}

		public void Disable(IStatReceiver receiver) {
			disableAction.Invoke(receiver);
		}

		public IEnumerable<AbstractSerializableStat> SuppliedStats() {
			return suppliedStats;
		}
	}
}
