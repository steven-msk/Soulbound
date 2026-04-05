using Brigadier.NET.Suggestion;
using System.Collections.Generic;

#nullable enable

namespace SoulboundEngine.Client.Debug.Commands {
	public sealed class CommandCompletion {
		private List<Suggestion> completions = new();
		private int selectedIndex = -1;

		public void SetCompletions(List<Suggestion> values) {
			this.completions = values;
			selectedIndex = completions.Count > 0 ? 0 : -1;
		}

		public int SelectNext() {
			if (completions.Count == 0) return -1;
			selectedIndex = (selectedIndex + 1) % completions.Count;
			return selectedIndex;
		}

		public int SelectPrevious() {
			if (completions.Count == 0) return -1;
			selectedIndex = (selectedIndex - 1 + completions.Count) % completions.Count;
			return selectedIndex;
		}

		public Suggestion? GetSelected() {
			return selectedIndex >= 0
				? completions[selectedIndex]
				: null;
		}

		public int GetSelectedIndex() => selectedIndex;
		public int GetCompletionCount() => completions.Count;
		public void ClearCompletions() => completions.Clear();
	}
}
