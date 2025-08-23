using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Flags]
public enum StatApplicationType {
	Flat				= 1 << 0,
	Percentage			= 1 << 1,
	FlatAndPercentage	= Flat | Percentage
}

public static class StatApplicationTypeValidation {
	public static bool Supports(this StatApplicationType allowed, StatApplicationType test) {
		return (allowed & test) == test;
	}
}
