using SoulboundBackend.Client.Stats;
using SoulboundBackend.Common;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;

#nullable enable

namespace SoulboundBackend.Client.ItemSystem {
	public abstract class StatItem : Item, IStatModificationSource {
		public abstract bool applyInstantStatsOnHoverOrSelect { get; }
		public abstract ModificationToken token { get; }
		public abstract IEnumerable<StatModificationPackage> GetPackages();
		[Obsolete]
		public ContextHandle<IStatReceiver> contextHandle { get; } = new();
		[Obsolete]
		protected bool hasContext => contextHandle.hasContext;
	}
}