using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class StatItem : Item, IStatProvider {
	public abstract bool ApplyInstantStatsAutomatically { get; }

	public abstract List<SerializableStat> InstantStats { get; }

	public abstract List<BufferedStat> BufferedStats { get; }

	protected override CompoundTooltip GetDefaultTooltip() {

		// TODO: figure out a way to interpolate Buffered and Instant stats onto the item
		return base.GetDefaultTooltip().Concat(Tooltip.Stats(InstantStats)); 
	}
}