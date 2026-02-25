using SoulboundBackend.Core.Debug.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core.Debug.Commands {
	public sealed class GlobalCommandProvider : ICommandProvider {
		private CommandNode prototypeCommand;

		public IEnumerable<CommandNode> GetCommands() {
			yield return Prototype();
		}

		private CommandNode Prototype() {
			if (prototypeCommand != null) return prototypeCommand;

			CommandBuilder command = CommandBuilder.Literal("command");
			command.Then(new LiteralCommandNode("subcommand1"))
				.Then(new LiteralCommandNode("anotherSubCommand"))
				.Executes(_ => Logger.LogInfo("executing anotherSubCommand"));
			command.Then(new LiteralCommandNode("subcommand"))
				.Executes(_ => Logger.LogInfo("subcommand2"));
			command.Then(new LiteralCommandNode("subcommand3"))
				.Executes(_ => Logger.LogInfo("impossible to run"));
			command.Then(new LiteralCommandNode("subcommand3"))
				.Executes(_ => Logger.LogInfo("impossible to run"));
			prototypeCommand = command.GetRootNode();
			return prototypeCommand;
		}

		
	}
}
