using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

public interface IStatActivator {
	public event Action<IStatSource>? OnActivated;
	public event Action<IStatSource>? OnDeactivated;


	public IStatEffectHandler SuppliedEffectHandler();
	public void Start(IStatSource source);
	public void Discard(IStatSource source);
}
