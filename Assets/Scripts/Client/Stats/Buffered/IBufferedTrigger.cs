using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IBufferedTrigger {
	public BufferedTriggerState State { get; set; }

	public Func<bool> InvocationValidator { get; }

	public void Enable(IBufferedStatImpl stat, IStatProvider source);

	public void Disable(IBufferedStatImpl stat, IStatProvider source);

	public void Invoke(IBufferedStatImpl stat, Action action);

	public bool ValidateExecution(IBufferedStatImpl stat, IStatProvider source, bool log);
}
