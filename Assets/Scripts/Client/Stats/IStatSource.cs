using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IStatSource {
	public void ApplyProvider(IStatProvider provider);

	public void RevokeProvider(IStatProvider provider);
}
