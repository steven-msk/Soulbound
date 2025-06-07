using System.Collections.Generic;

public enum TooltipStatPattern {
	ValueFirst,
	NameFirst
}

public static class TooltipStatPatternApplicator {
	public static string GetPattern(this TooltipStatPattern pattern, KeyValuePair<string, object> stat) {
		return pattern switch {
			TooltipStatPattern.ValueFirst => stat.Value.ToString() + " " + stat.Key,
			TooltipStatPattern.NameFirst => stat.Key + " " + stat.Value.ToString(),
			_ => throw new System.NotImplementedException()
		};
	}
}