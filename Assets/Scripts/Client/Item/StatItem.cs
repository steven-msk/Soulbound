using SoulboundBackend.Client.Stats;
using SoulboundBackend.Common;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;

#nullable enable

namespace SoulboundBackend.Client.ItemSystem {
	public abstract class StatItem : Item, IStatModificationSource {
		private readonly ModificationToken _token = new();
		public virtual ModificationToken token => _token;
		public abstract IEnumerable<StatModificationPackage> GetPackages();
	}
}