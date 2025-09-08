using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

public interface IStatSource {

	public void ApplyStats(IEnumerable<AbstractSerializableStat> stats, IStatProvider provider);

	public void RevokeStats(IEnumerable<AbstractSerializableStat> stats, IStatProvider provider);
}
