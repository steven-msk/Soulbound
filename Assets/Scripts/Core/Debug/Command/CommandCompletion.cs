using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundBackend.Core.Debug.Commands {
	public sealed class CommandCompletion {
		private List<string> completions = new();
		private int selectedIndex;

		public void SetCompletions(IEnumerable<string> values) {
			completions = values.ToList();
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

		public string? GetSelected() {
			return selectedIndex >= 0
				? completions[selectedIndex]
				: null;
		}
	}
}
