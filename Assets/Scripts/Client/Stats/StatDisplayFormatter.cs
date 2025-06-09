using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class StatDisplayFormatter {
	public static Func<TValue, string, string> PlainNameFormat<TValue>() => (value, baseName) => baseName;

	public static Func<TValue, string, string> PluralAdaptedNameFormat<TValue>() => (value, baseName) => Convert.ToDouble(value) > 1 ? $"{baseName}s" : baseName;

	public static Func<float, string> PercentageValueFormat() => value => $"{value * 100}%";

	public static Func<TValue, string> PlainValueFormat<TValue>() => value => value.ToString();
}