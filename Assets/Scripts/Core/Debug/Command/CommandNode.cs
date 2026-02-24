using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core.Debug.Commands {
	public abstract class CommandNode {
		public abstract string label { get; }
		protected List<CommandNode> children = new();
		protected readonly bool isExecutable;

		protected CommandNode(bool isExecutable) {
			this.isExecutable = isExecutable; 
		}

		public abstract bool Matches(string token, CommandContext context);

		public void AddChild(CommandNode child) => children.Add(child);
		public IEnumerable<CommandNode> GetChildren() {
			for (int i = 0; i < children.Count; i++) {
				yield return children[i];
			}
		}

		public bool HasChildren() => children.Any();

		public bool IsExecutable() => isExecutable;
	}
}
