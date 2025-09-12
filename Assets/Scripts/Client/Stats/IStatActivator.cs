using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

public interface IStatActivator {
	public event Action<IStatReceiver>? OnActivated;
	public event Action<IStatReceiver>? OnDeactivated;

	public IEnumerable<IStatEffectHandler> SuppliedEffectHandlers();
	public void Start(IStatReceiver receiver);
	public void Discard(IStatReceiver receiver);
}
