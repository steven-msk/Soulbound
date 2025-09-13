using System.Collections.Generic;

#nullable enable

public interface IStatReceiver {

	public void ApplyStats(IEnumerable<AbstractSerializableStat> stats, IStatProvider provider);

	public void RevokeStats(IEnumerable<AbstractSerializableStat> stats, IStatProvider provider);
}
