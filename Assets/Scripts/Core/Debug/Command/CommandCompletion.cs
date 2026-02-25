using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundBackend.Core.Debug.Commands {
	public sealed class CommandCompletion {
		private List<CommandCompletionToken> completions = new();
		private int selectedIndex = -1;

		public void SetCompletions(List<CommandCompletionToken> values) {
			this.completions = values;
			selectedIndex = completions.Count > 0 ? 0 : -1;
		}

		public void SelectNext() {
			if (completions.Count == 0) return;
			selectedIndex = (selectedIndex + 1) % completions.Count;
		}

		public void SelectPrevious() {
			if (completions.Count == 0) return;
			selectedIndex = (selectedIndex - 1 + completions.Count) % completions.Count;
		}

		public CommandCompletionToken? GetSelected() {
			return selectedIndex >= 0
				? completions[selectedIndex]
				: null;
		}

		public int GetSelectedIndex() => selectedIndex;
	}
}
