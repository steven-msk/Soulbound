using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IStatProvider : IItemCapability {
	public bool ApplyStatsAutomatically { get; }
	public List<SerializableStat> Stats { get; }

	public virtual void ApplyStats(PlayerController player) {
		player.Stats.Apply(Stats);
	}
}