using SoulboundBackend.Core.Debug.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core.Debug.Commands {
	public sealed class WorldSessionCommands : ICommandProvider {
		private readonly CommandNode teleport;

		public WorldSessionCommands() {
			teleport = CreateTeleportCommand();
		}

		public IEnumerable<CommandNode> GetCommands() {
			yield return teleport;
		}

		private CommandNode CreateTeleportCommand() {
			if (teleport != null) return teleport;

			CommandBuilder coords = CommandBuilder.Create(new ArgumentCommandNode<Coordinate>("x", new CoordinateParser()))
				.Then(new ArgumentCommandNode<Coordinate>("y", new CoordinateParser()))
				.Executes(ctx => {
					Guid targetGuid = ctx.Args.TryGet("target", out Guid guid)
					   ? guid
					   : ctx.Data.Player.GetGuid();

					var target = ctx.Data.Entities.TryGetEntity(targetGuid, out var entity)
						? entity
						: ctx.Data.Player;

					var pos = target.GetPos();

					var xcoord = ctx.Args.Get<Coordinate>("x");
					var ycoord = ctx.Args.Get<Coordinate>("y");

					float x = xcoord.isRelative ? pos.x + xcoord.value : xcoord.value;
					float y = ycoord.isRelative ? pos.y + ycoord.value : ycoord.value;

					ctx.ExecServices.Player.SetPos(new UnityEngine.Vector2(x, y));
					Logger.LogInfo("teleported {} to x:{} y:{}", target, x, y);
				});

			CommandBuilder tp = CommandBuilder.Literal("tp");
			tp.Then(coords);
			tp.Then(new EntityArgumentCommandNode("target"))
				.Then(coords);
			return tp.GetRootNode();
		}
	}
}
