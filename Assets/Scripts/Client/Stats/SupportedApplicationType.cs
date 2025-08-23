using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Flags]
public enum SupportedApplicationType {
	Flat				= 1 << 0,
	Percentage			= 1 << 1,
	FlatAndPercentage	= Flat | Percentage,
}

public static class SupportedApplicationTypeValidation {
	public static bool Supports(this SupportedApplicationType type, StatApplicationType test) {
		return type.HasFlag((SupportedApplicationType)(int)test);
	}
}
