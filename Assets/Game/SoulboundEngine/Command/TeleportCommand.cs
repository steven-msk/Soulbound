namespace SoulboundEngine.Command {
	using Brigadier.NET.Builder;
	using Brigadier.NET.Context;
	using Brigadier.NET.Tree;
	using SoulboundEngine.Common;
	using SoulboundEngine.Common.Math;
	using SoulboundEngine.World.Entity;
	using System;

#nullable enable

	public class TeleportCommand<TContext> : RegisterableCommand<TContext> where TContext : ICommandContext {
		public override void Supply(Func<LiteralArgumentBuilder<TContext>, LiteralCommandNode<TContext>> supplier, Action<string> outputConsumer) {
			supplier(LiteralArgumentBuilder<TContext>.LiteralArgument("tp")
				.Then(c => c.Argument("x", new CoordinateArgumentType())
					.Then(c => c.Argument("y", new CoordinateArgumentType())
						.Executes(ctx => this.Teleport(ctx.Source.Get(level => level.GetPlayer().guid), ctx, outputConsumer))
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

		public virtual int Teleport(Guid guid, CommandContext<TContext> ctx, Action<string> outputConsumer) {
			Entity? entity = ctx.Source.Get(level => level.GetEntity(guid));
			if (entity == null) return -1;

			Vec2d pos = entity.GetPosition();
			Vec2d target = ctx.Source.Get(level => level.GetPlayer().GetWorldPointerPos());
			double x = ctx.GetArgument<Coordinate>("x").GetPos(pos.x, target.x);
			double y = ctx.GetArgument<Coordinate>("y").GetPos(pos.y, target.y);

			ctx.Source.Run(level => {
				entity.SetPos(x, y);
				outputConsumer("Teleported {} to ({}, {})".WithArgs(target, x, y));
			});
			return 1;
		}
	}
}
