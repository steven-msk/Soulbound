using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

public sealed class StatActivator_test : IStatActivator {
	public event Action<IStatSource>? OnActivated;
	public event Action<IStatSource>? OnDeactivated;
	private IStatEffectHandler effectHandler;

	public StatActivator_test(IStatEffectHandler effectHandler) {
		this.effectHandler = effectHandler;
		OnActivated += effectHandler.Enable;
		OnDeactivated += effectHandler.Disable;
	}

	public void Start(IStatSource source) {
		OnActivated?.Invoke(source);
	}

	public void Discard(IStatSource source) {
		OnDeactivated?.Invoke(source);
	}

	public IStatEffectHandler SuppliedEffectHandler() {
		return effectHandler;
	}
}
