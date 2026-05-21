using Brigadier.NET.Suggestion;
using System.Collections.Generic;

#nullable enable

namespace SoulboundEngine.Client.Debug.Commands {
	public sealed class CompletionManager {
		private List<Suggestion> completions = new();
		private int selectedIndex = -1;

		public void SetCompletions(List<Suggestion> values) {
			this.completions = values;
			this.selectedIndex = this.completions.Count > 0 ? 0 : -1;
		}

		public int SelectNext() {
			if (this.completions.Count == 0) return -1;
			this.selectedIndex = (this.selectedIndex + 1) % this.completions.Count;
			return this.selectedIndex;
		}

		public int SelectPrevious() {
			if (this.completions.Count == 0) return -1;
			this.selectedIndex = (this.selectedIndex - 1 + this.completions.Count) % this.completions.Count;
			return this.selectedIndex;
		}

		public Suggestion? GetSelected() {
			return this.selectedIndex >= 0
				? this.completions[this.selectedIndex]
				: null;
		}

		public int GetSelectedIndex() => this.selectedIndex;

		public int GetCompletionCount() => this.completions.Count;

		public void ClearCompletions() => this.completions.Clear();

		public Suggestion Get(int index) => this.completions[index];
	}
}
