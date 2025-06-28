using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IBufferedTrigger {
	public void Enable(BufferedStat stat);

	public void Disable(BufferedStat stat);

	protected Func<bool> InvocationValidator { get; }

	public void Invoke(BufferedStat stat, Action action);
}
