namespace SoulboundEngine.Command {
	using Brigadier.NET.Builder;
	using Brigadier.NET.Context;
	using Brigadier.NET.Tree;
	using SoulboundEngine.Common;
	using SoulboundEngine.Common.Math;
	using SoulboundEngine.World.Block;
	using System;

	public class SetblockCommand<TContext> : RegisterableCommand<TContext> where TContext : ICommandContext {
		public override void Supply(Func<LiteralArgumentBuilder<TContext>, LiteralCommandNode<TContext>> supplier, Action<string> outputConsumer) {
			supplier(LiteralArgumentBuilder<TContext>.LiteralArgument("setblock")
				.Then(c => c.Argument("x", new CoordinateArgumentType())
					.Then(c => c.Argument("y", new CoordinateArgumentType())
						.Then(c => c.Argument("block", new BlockArgumentType())
							.Executes(ctx => this.Execute(ctx, outputConsumer))
						)
					)
				)
			);
		}

		public virtual int Execute(CommandContext<TContext> ctx, Action<string> outputConsumer) {
			Block block = ctx.GetArgument<Block>("block");
			Vec2d playerPos = ctx.Source.Get(level => level.GetPlayer().GetPosition());
			Vec2d target = ctx.Source.Get(level => level.GetPlayer().GetWorldPointerPos());
			BlockPos blockPos = new() {
				x = Maths.FloorToInt(ctx.GetArgument<Coordinate>("x").GetPos(playerPos.x, target.x)),
				y = Maths.FloorToInt(ctx.GetArgument<Coordinate>("y").GetPos(playerPos.y, target.y))
			};
			ctx.Source.Run(level => {
				level.SetBlockState(blockPos, block.DefaultState);
				outputConsumer("Reset block {} at {}".WithArgs(Blocks.GetIdentifier(block), blockPos));
			});
			return 1;
		}
	}
}
