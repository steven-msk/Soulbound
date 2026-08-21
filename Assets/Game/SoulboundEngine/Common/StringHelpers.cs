namespace SoulboundEngine.Common {
	public static class StringHelpers {
		public static string ReplaceFirst(this string text, string search, string replace) {
			int pos = text.IndexOf(search);
			return pos < 0 ? text : text[..pos] + replace + text[(pos + search.Length)..];
		}
	}
}
