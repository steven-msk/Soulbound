using SoulboundBackend.Client.ItemSystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoulboundBackend.Client.Stats {
	[Obsolete]
	public interface IStatEffectHandler {
		public IEnumerable<AbstractValueModifier> SuppliedStats();
		public void Enable(IStatReceiver receiver);
		public void Disable(IStatReceiver receiver);

		public static IEnumerable<AbstractValueModifier> SupplyStats(params AbstractValueModifier[] stats) {
			return StatSupplier.Of(stats);
		}

		public static IStatEffectHandler Static(IStatProvider provider, params AbstractValueModifier[] stats) {
			return new DelegatedStatEffectHandler(
				receiver => receiver.ApplyStats(stats, provider),
				receiver => receiver.RevokeStats(stats, provider), 
				stats
			);
		}

		public static IStatEffectHandler Timed(float durationSeconds, bool resetOnEnable, IStatProvider provider, params AbstractValueModifier[] stats) {
			return new TimedStatEffectHandler(provider, stats, durationSeconds, resetOnEnable);
		}

		public class StatSupplier {
			public List<AbstractValueModifier> stats { get; private set; } = new();

			public static List<AbstractValueModifier> Of(params AbstractValueModifier[] stats) {
				return stats.ToList();
			}

			public StatSupplier Add(params AbstractValueModifier[] stats) {
				this.stats.AddRange(stats);
				return this;
			}

			public StatSupplier Set(params AbstractValueModifier[] stats) {
				this.stats = stats.ToList();
				return this;
			}

			public List<AbstractValueModifier> Finish() {
				return stats;
			}
		}
	}
}