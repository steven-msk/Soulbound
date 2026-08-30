namespace SoulboundEngine.Common {
	public static class StringHelpers {
		public static string ReplaceFirst(this string text, string search, string replace) {
			int pos = text.IndexOf(search);
			return pos < 0 ? text : text[..pos] + replace + text[(pos + search.Length)..];
		}

		public static string WithArgs(this string text, string argMarker, object[] args) {
			if (string.IsNullOrEmpty(text) || args == null || args.Length == 0) {
				return text;
			}

			int argIndex = 0;
			while (text.Contains(argMarker) && argIndex < args.Length) {
				text = text.ReplaceFirst(argMarker, args[argIndex]?.ToString() ?? "null");
				argIndex++;
			}
			return text;
		}

		public static string WithArgs(this string text, params object[] args) {
			return WithArgs(text, "{}", args);
		}
	}
}
