using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IStatEffectHandler {
	public void Enable(IStatSource source);
	public void Disable(IStatSource source);
}