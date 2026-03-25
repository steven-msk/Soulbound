using System.Linq;
using System.Text;
using UnityEngine;

namespace SoulboundBackend.Client.Debug.Commands {
	public static class CommandFormat {
		public const string MARKER_FORMAT = ">>{0}<<";

		public static string FormatWhere(string[] tokens, int tokenIndex, string format = MARKER_FORMAT) {
			StringBuilder builder = new();

			int startPrefix = Mathf.Max(0, tokenIndex - 3);
			builder.AppendJoin(' ', tokens[startPrefix..tokenIndex]);

			builder.Append(' ').AppendFormat(format, tokens[tokenIndex]);

			int endSuffix = Mathf.Min(tokens.Length, tokenIndex + 3);
			builder.AppendJoin(' ', tokens[(tokenIndex + 1)..endSuffix]);

			return builder.ToString();
		}

		public static string FormatQuoting(string[] entries) {
			return string.Join(", ", entries.Select(s => $"'{s}'"));
		}
	}
}
