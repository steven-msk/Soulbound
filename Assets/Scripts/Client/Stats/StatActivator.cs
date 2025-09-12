using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

#nullable enable

public class StatActivator {
	public event Action<IStatReceiver>? OnActivated;
	public event Action<IStatReceiver>? OnDeactivated;
	public IEnumerable<IStatEffectHandler> effectHandlers { get; private set; }

	public StatActivator(params IStatEffectHandler[] effectHandlers) {
		this.effectHandlers = effectHandlers;
		OnActivated += (receiver) => effectHandlers.ToList().ForEach(h => h.Enable(receiver));
		OnDeactivated += (receiver) => effectHandlers.ToList().ForEach(h => h.Disable(receiver));
	}

	public StatActivator(Action<Action<IStatReceiver>>? activationBinder, Action<Action<IStatReceiver>>? deactivationBinder, 
			params IStatEffectHandler[] effectHandlers) 
		: this(effectHandlers) {
		activationBinder?.Invoke(BeginContext);
		deactivationBinder?.Invoke(EndContext);
	}

	public void BeginContext(IStatReceiver receiver) {
		OnActivated?.Invoke(receiver);
	}

	public void EndContext(IStatReceiver receiver) {
		OnDeactivated?.Invoke(receiver);
	}
}
