using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class StatItem : Item, IStatProvider {
	public abstract bool ApplyInstantStatsAutomatically { get; }

	public abstract List<SerializableStat> InstantStats { get; }

	public abstract List<BufferedStat> BufferedStats { get; }

	protected override CompoundTooltip GetDefaultTooltip() => base.GetDefaultTooltip().Concat(Tooltip.Stats(InstantStats));
}