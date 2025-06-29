using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IStatProvider : IItemCapability {
	public bool ApplyInstantStatsAutomatically { get; }
	public List<SerializableStat> InstantStats { get; }
	public List<BufferedStat> BufferedStats { get; }

	// FEATUREIMPL (WIP): buffered stats - NOT TESTED

	public virtual void ApplyInstantStats(PlayerStats playerStats) => playerStats.Apply(InstantStats, this);

	public virtual void RevokeInstantStats(PlayerStats platerStats) => platerStats.Revoke(InstantStats, this);

	public virtual void EnableBuffers(PlayerStats playerStats) {
		BufferedStats.ForEach(bufferedStat => bufferedStat.EnableBuffers(this));
	}

	public virtual void DisableBuffers(PlayerStats playerStats) {
		BufferedStats.ForEach(bufferedStat => bufferedStat.DisableBuffers(this));
	}
}