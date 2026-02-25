using SoulboundBackend.Core.Debug.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core.Debug.Commands {
	public sealed class CommandProvider : ICommandProvider {
		private CommandNode prototypeCommand;
		private CommandNode teleport;

		public IEnumerable<CommandNode> GetCommands() {
			yield return Prototype();
			yield return Teleport();
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

		private CommandNode Teleport() {
			if (teleport != null) return teleport;

			CommandBuilder coords = CommandBuilder.Create(new ArgumentCommandNode<Coordinate>("x", new CoordinateParser()))
				.Then(new ArgumentCommandNode<Coordinate>("y", new CoordinateParser()))
				.Executes(ctx => {
					string target = ctx.Args.TryGet("target", out Guid guid)
						? guid.ToString()
						: "player";
					Coordinate x = ctx.Args.Get<Coordinate>("x");
					Coordinate y = ctx.Args.Get<Coordinate>("y");
					Logger.LogInfo("teleported {} to x:{} y:{}", target, x.value, y.value);
				});

			CommandBuilder tp = CommandBuilder.Literal("tp");
			tp.Then(coords);
			tp.Then(new ArgumentCommandNode<Guid>("target", new GuidParser()))
				.Then(coords);
			teleport = tp.GetRootNode();
			return teleport;
		}
	}
}
