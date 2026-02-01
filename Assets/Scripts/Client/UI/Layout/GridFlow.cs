using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.UI {
	public enum GridFlow {
		RowMajor,		// left -> right, then push down
		ColumnMajor,	// top -> bottom, then push right
	}
}
