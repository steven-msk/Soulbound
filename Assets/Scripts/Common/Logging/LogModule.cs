using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct LogModule {
	public string header;
	public string colorCode;

	public LogModule(string header, string colorCode) {
		this.header = header;
		this.colorCode = colorCode;
	}

	public string FormatHeaders() => $"<color={colorCode}>[{header}]</color>";
}
