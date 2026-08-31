namespace SoulboundEngine.Command {
	using Brigadier.NET.Builder;
	using Brigadier.NET.Context;
	using Brigadier.NET.Tree;
	using SoulboundEngine.Common;
	using SoulboundEngine.Common.Math;
	using SoulboundEngine.World.Entity;
	using System;

#nullable enable

	public class SpawnCommand<TContext> : RegisterableCommand<TContext> where TContext : ICommandContext {
		public override void Supply(Func<LiteralArgumentBuilder<TContext>, LiteralCommandNode<TContext>> supplier, Action<string> outputConsumer) {
			supplier(LiteralArgumentBuilder<TContext>.LiteralArgument("spawn")
				.Then(c => c.Argument("entityType", new EntityDescriptorArgumentType())
					.Executes(ctx => {
						Vec2d pos = ctx.Source.Get(level => level.GetPlayer().GetPosition());
						Coordinate x = new(false, pos.x, false);
						Coordinate y = new(false, pos.y, false);
						return this.SpawnEntity(x, y, ctx, outputConsumer);
					})

					.Then(c => c.Argument("x", new CoordinateArgumentType())
						.Then(c => c.Argument("y", new CoordinateArgumentType())
							.Executes(ctx => {
								Coordinate x = ctx.GetArgument<Coordinate>("x");
								Coordinate y = ctx.GetArgument<Coordinate>("y");
								return this.SpawnEntity(x, y, ctx, outputConsumer);
							})
						)
					)
				)
			);
		}

		public virtual int SpawnEntity(Coordinate x, Coordinate y, CommandContext<TContext> ctx, Action<string> outputConsumer) {
			EntityDescriptor entityDescriptor = ctx.GetArgument<EntityDescriptor>("entityType");
			Vec2d relativePos = ctx.Source.Get(level => level.GetPlayer().GetPosition());
			Vec2d target = ctx.Source.Get(level => level.GetPlayer().GetWorldPointerPos());
			ctx.Source.Run(level => {
				Vec2d pos = new(x.GetPos(relativePos.x, target.x), y.GetPos(relativePos.y, target.y));
				if (level.SpawnEntity(entityDescriptor, pos)) {
					outputConsumer("Spawned new {} at {}".WithArgs(entityDescriptor, pos));
				}
			});
			return 1;
		}
	}
}
