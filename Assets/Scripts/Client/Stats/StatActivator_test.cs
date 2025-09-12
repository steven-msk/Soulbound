using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

public sealed class StatActivator_test : IStatActivator {
	public event Action<IStatReceiver>? OnActivated;
	public event Action<IStatReceiver>? OnDeactivated;
	private IEnumerable<IStatEffectHandler> effectHandlers;

	public StatActivator_test(params IStatEffectHandler[] effectHandlers) {
		this.effectHandlers = effectHandlers;
		OnActivated += (receiver) => effectHandlers.ToList().ForEach(h => h.Enable(receiver));
		OnDeactivated += (receiver) => effectHandlers.ToList().ForEach(h => h.Disable(receiver));
	}

	public void Start(IStatReceiver receiver) {
		OnActivated?.Invoke(receiver);
	}

	public void Discard(IStatReceiver receiver) {
		OnDeactivated?.Invoke(receiver);
	}

	public IEnumerable<IStatEffectHandler> SuppliedEffectHandlers() {
		return effectHandlers;
	}
}
