using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IStatProvider : IItemCapability {
	public bool applyInstantStatsOnHoverOrSelect { get; }
	public List<AbstractSerializableStat> instantStats { get; }
	public List<IBufferedStatImpl> bufferedStats { get; }

	// FEATUREIMPL (WIP): buffered stats - NOT TESTED

	public virtual void ApplyInstantStats(PlayerStats playerStats) => playerStats.Apply(instantStats, this);

	public virtual void RevokeInstantStats(PlayerStats platerStats) => platerStats.Revoke(instantStats, this);

	public virtual void EnableBuffers(PlayerStats playerStats) {
		bufferedStats.ForEach(bufferedStat => bufferedStat.EnableBuffers(this));
	}

	public virtual void DisableBuffers(PlayerStats playerStats) {
		bufferedStats.ForEach(bufferedStat => bufferedStat.DisableBuffers(this));
	}
}