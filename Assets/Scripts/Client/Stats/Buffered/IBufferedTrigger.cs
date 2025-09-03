using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Obsolete]
public interface IBufferedTrigger {
	public Func<bool> InvocationValidator { get; }

	public void Enable(IBufferedStatImpl stat, IStatProvider provider, BufferedTriggerState state);

	public void Disable(IBufferedStatImpl stat, IStatProvider provider, BufferedTriggerState state);

	public void Invoke(IBufferedStatImpl stat, Action action);

	public bool ValidateExecution(IBufferedStatImpl stat, IStatProvider provider, bool log);
}
