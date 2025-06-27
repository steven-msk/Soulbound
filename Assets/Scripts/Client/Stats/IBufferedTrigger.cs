using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IBufferedTrigger {
	public void Bind(BufferedStat bufferedStat, Action apply, Action revoke);

	public void Unbind();
}
