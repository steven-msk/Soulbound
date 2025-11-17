using SoulboundBackend.Client.Stats;
using SoulboundBackend.Common;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;

#nullable enable

namespace SoulboundBackend.Client.ItemSystem {
	public abstract class StatItem : Item, IStatProvider {
		public abstract bool applyInstantStatsOnHoverOrSelect { get; }

		public abstract IEnumerable<StatMapping> statMappings { get; }

		public ContextHandle<IStatReceiver> contextHandle { get; } = new();
		protected bool hasContext => contextHandle.hasContext;

		protected StatItem() => StaticResetManager.Register(this);

		public override void StaticReset() => contextHandle.hasContext = false;
	}
}