namespace SoulboundEngine.UnityClient.Debug.Command {
	using Brigadier.NET.Builder;
	using Brigadier.NET.Context;
	using Brigadier.NET.Tree;
	using SoulboundEngine.Common;
	using SoulboundEngine.Common.Math;
	using SoulboundEngine.World.Entity;
	using System;

	public record TeleportCommand : IRegisterableCommand {
		public void Supply(Func<LiteralArgumentBuilder<RuntimeCommandSource>, LiteralCommandNode<RuntimeCommandSource>> supplier, Action<string> outputConsumer) {
			supplier(LiteralArgumentBuilder<RuntimeCommandSource>.LiteralArgument("tp")
				.Then(c => c.Argument("x", new CoordinateArgumentType())
					.Then(c => c.Argument("y", new CoordinateArgumentType())
						.Executes(ctx => this.Teleport(ctx.Source.data.Player.GetGuid(), ctx, outputConsumer))
					)
				).Then(c => c.Argument("target", new GuidArgumentType())
					.Then(c => c.Argument("x", new CoordinateArgumentType())
						.Then(c => c.Argument("y", new CoordinateArgumentType())
							.Executes(ctx => this.Teleport(ctx.GetArgument<Guid>("target"), ctx, outputConsumer))
						)
					)
				)
			);
		}

		public int Teleport(Guid guid, CommandContext<RuntimeCommandSource> ctx, Action<string> outputConsumer) {
			if (!ctx.Source.data.Entities.TryGetEntity(guid, out IEntityView target)) {
				return -1;
			}

			Vec2d pos = target.GetPos();
			double x = ctx.GetArgument<Coordinate>("x").GetPos(pos.x);
			double y = ctx.GetArgument<Coordinate>("y").GetPos(pos.y);

			ctx.Source.execServices.Entity.SetPos(target.GetGuid(), new Vec2d(x, y));
			outputConsumer("Teleported {} to ({}, {})".WithArgs(target, x, y));

			return 1;
		}
	}
}
