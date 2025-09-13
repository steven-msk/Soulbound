using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

public class StatActivator {
	public IEnumerable<IStatEffectHandler> effectHandlers { get; private set; }

	public StatActivator(params IStatEffectHandler[] effectHandlers) {
		this.effectHandlers = effectHandlers;
	}

	public StatActivator(Action<Action<IStatReceiver>>? activationBinder, Action<Action<IStatReceiver>>? deactivationBinder, 
			params IStatEffectHandler[] effectHandlers) 
		: this(effectHandlers) {
		activationBinder?.Invoke(BeginContext);
		deactivationBinder?.Invoke(EndContext);
	}

	public void BeginContext(IStatReceiver receiver) {
		effectHandlers.ToList().ForEach(h => h.Enable(receiver));
	}

	public void EndContext(IStatReceiver receiver) {
		effectHandlers.ToList().ForEach(h => h.Disable(receiver));
	}
}
