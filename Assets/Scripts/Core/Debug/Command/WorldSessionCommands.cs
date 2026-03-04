using Assets.Scripts.Core.Debug.Command;
using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Core.Debug.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Logger = SoulboundBackend.Core.Debug.Logging.Logger;

namespace SoulboundBackend.Core.Debug.Commands {
	public sealed class WorldSessionCommands : ICommandProvider {
		private readonly CommandNode teleport;
		private readonly CommandNode setblock = CommandBuilder.Literal("setblock")
				.ThenCursorOf(Coords2D())
				.Then(new BlockArgumentCommandNode("block"))
				.Executes(ctx => {
					Block block = ctx.Args.Get<Block>("block");
					Vector2 playerPos = ctx.Data.Player.GetPos();
					BlockPos blockPos = new(
						Mathf.FloorToInt(ctx.Args.Get<Coordinate>("x").GetPos(playerPos.x)),
						Mathf.FloorToInt(ctx.Args.Get<Coordinate>("y").GetPos(playerPos.y))
					);
					ctx.ExecServices.Level.SetBlockState(blockPos, block.defaultState);
					Logger.LogInfo("Set block {} at {}", block.id, blockPos);
				})
				.GetRootNode();

		public WorldSessionCommands() {
			teleport = TeleportCommand();
		}

		public IEnumerable<CommandNode> GetCommands() {
			yield return teleport;
			yield return setblock;
		}

		private static CommandBuilder Coords2D() {
			return CommandBuilder.Create(new ArgumentCommandNode<Coordinate>("x", new CoordinateParser()))
				.Then(new ArgumentCommandNode<Coordinate>("y", new CoordinateParser()));
		}

		private CommandNode TeleportCommand() {
			CommandBuilder coords = Coords2D()
				.Executes(ctx => {
					Guid targetGuid = ctx.Args.TryGet("target", out Guid guid)
					   ? guid
					   : ctx.Data.Player.GetGuid();

					IEntityView target = ctx.Data.Entities.TryGetEntity(targetGuid, out var entity)
						? entity
						: ctx.Data.Player;

					Vector2 pos = target.GetPos();

					Coordinate xcoord = ctx.Args.Get<Coordinate>("x");
					Coordinate ycoord = ctx.Args.Get<Coordinate>("y");

					float x = xcoord.GetPos(pos.x);
					float y = ycoord.GetPos(pos.y);

					ctx.ExecServices.Entity.SetPos(targetGuid, new Vector2(x, y));
					Logger.LogInfo("teleported {} to x:{} y:{}", target, x, y);
				});

			CommandBuilder tp = CommandBuilder.Literal("tp");
			tp.ThenRootOf(coords);
			tp.Then(new EntityArgumentCommandNode("target"))
				.ThenRootOf(coords);
			return tp.GetRootNode();
		}
	}
}
