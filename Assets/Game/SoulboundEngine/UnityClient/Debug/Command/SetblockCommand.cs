namespace SoulboundEngine.UnityClient.Debug.Command {
	using Brigadier.NET.Builder;
	using Brigadier.NET.Context;
	using Brigadier.NET.Tree;
	using SoulboundEngine.Common;
	using SoulboundEngine.Common.Math;
	using SoulboundEngine.World.Block;
	using System;

	public record SetblockCommand : IRegisterableCommand {
		public void Supply(Func<LiteralArgumentBuilder<RuntimeCommandSource>, LiteralCommandNode<RuntimeCommandSource>> supplier, Action<string> outputConsumer) {
			supplier(LiteralArgumentBuilder<RuntimeCommandSource>.LiteralArgument("setblock")
				.Then(c => c.Argument("x", new CoordinateArgumentType())
					.Then(c => c.Argument("y", new CoordinateArgumentType())
						.Then(c => c.Argument("block", new BlockArgumentType())
							.Executes(ctx => this.Execute(ctx, outputConsumer))
						)
					)
				)
			);
		}

		public int Execute(CommandContext<RuntimeCommandSource> ctx, Action<string> outputConsumer) {
			Block block = ctx.GetArgument<Block>("block");
			Vec2d playerPos = ctx.Source.data.Player.GetPos();
			BlockPos blockPos = new() {
				x = Maths.FloorToInt(ctx.GetArgument<Coordinate>("x").GetPos(playerPos.x)),
				y = Maths.FloorToInt(ctx.GetArgument<Coordinate>("y").GetPos(playerPos.y))
			};
			ctx.Source.execServices.Level.SetBlockState(blockPos, block.DefaultState);
			outputConsumer("Set block {} at {}".WithArgs(Blocks.GetIdentifier(block), blockPos));
			return 1;
		}
	}
}
