using System;
using System.Collections.Generic;

namespace SoulboundBackend.Core {
	[Obsolete]
	public static class StaticResetManager {
		private static readonly List<IStaticResettable> resettables = new();

		public static void Register(IStaticResettable resettable) {
			if (!resettables.Contains(resettable)) {
				resettables.Add(resettable);
			}
		}

		public static void ResetAll() {
			foreach (var resettable in resettables) {
				resettable.StaticReset();
			}
		}
	}
}
