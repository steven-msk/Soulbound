using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

public sealed class StatActivator_test : IStatActivator {
	public event Action<IStatSource>? OnActivated;
	public event Action<IStatSource>? OnDeactivated;
	private IEnumerable<IStatEffectHandler> effectHandlers;

	public StatActivator_test(params IStatEffectHandler[] effectHandlers) {
		this.effectHandlers = effectHandlers;
		OnActivated += (source) => effectHandlers.ToList().ForEach(h => h.Enable(source));
		OnDeactivated += (source) => effectHandlers.ToList().ForEach(h => h.Disable(source));
	}

	public void Start(IStatSource source) {
		OnActivated?.Invoke(source);
	}

	public void Discard(IStatSource source) {
		OnDeactivated?.Invoke(source);
	}

	public IEnumerable<IStatEffectHandler> SuppliedEffectHandlers() {
		return effectHandlers;
	}
}
