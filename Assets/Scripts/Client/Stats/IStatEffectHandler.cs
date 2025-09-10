using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

public interface IStatEffectHandler {
	public IEnumerable<AbstractSerializableStat> SuppliedStats();
	public void Enable(IStatSource source);
	public void Disable(IStatSource source);

	public static IEnumerable<AbstractSerializableStat> SupplyStats(params AbstractSerializableStat[] stats) {
		return StatSupplier.Of(stats);
	}

	public static IStatEffectHandler Static(IStatProvider provider, params AbstractSerializableStat[] stats) {
		return new DelegatedStatEffectHandler(source => source.ApplyStats(stats, provider), source => source.RevokeStats(stats, provider), stats);
	}

	public static IStatEffectHandler Timed(float durationSeconds, IStatProvider provider, params AbstractSerializableStat[] stats) {
		return new TimedStatEffectHandler(provider, stats, durationSeconds);
	}

	public class StatSupplier {
		public List<AbstractSerializableStat> stats { get; private set; } = new();

		public static List<AbstractSerializableStat> Of(params AbstractSerializableStat[] stats) {
			return stats.ToList();
		}

		public StatSupplier Add(params AbstractSerializableStat[] stats) {
			this.stats.AddRange(stats);
			return this;
		}

		public StatSupplier Set(params AbstractSerializableStat[] stats) {
			this.stats = stats.ToList();
			return this;
		}

		public List<AbstractSerializableStat> Finish() {
			return stats;
		}
	}
}