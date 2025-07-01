using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IBufferedTrigger {
	public BufferedTriggerState State { get; set; }

	public Func<bool> InvocationValidator { get; }

	public void Enable(BufferedStat stat, IStatProvider source);

	public void Disable(BufferedStat stat, IStatProvider source);

	public void Invoke(BufferedStat stat, Action action);
}
