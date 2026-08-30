namespace SoulboundEngine.UnityClient.Debug.Command {
	using Brigadier.NET.Builder;
	using Brigadier.NET.Context;
	using Brigadier.NET.Tree;
	using SoulboundEngine.Common;
	using SoulboundEngine.Common.Math;
	using SoulboundEngine.World.Entity;
	using System;

	public record SpawnCommand : IRegisterableCommand {
		public void Supply(Func<LiteralArgumentBuilder<RuntimeCommandSource>, LiteralCommandNode<RuntimeCommandSource>> supplier, Action<string> outputConsumer) {
			supplier(LiteralArgumentBuilder<RuntimeCommandSource>.LiteralArgument("spawn")
				.Then(c => c.Argument("entityType", new EntityDescriptorArgumentType())
					.Executes(ctx => this.SpawnEntity(false, ctx, outputConsumer))

					.Then(c => c.Argument("x", new CoordinateArgumentType())
						.Then(c => c.Argument("y", new CoordinateArgumentType())
							.Executes(ctx => this.SpawnEntity(true, ctx, outputConsumer))
						)
					)
				)
			);
		}

		public int SpawnEntity(bool specifiedPos, CommandContext<RuntimeCommandSource> ctx, Action<string> outputConsumer) {
			EntityDescriptor entityDescriptor = ctx.GetArgument<EntityDescriptor>("entityType");
			Vec2d pos = ctx.Source.data.Player.GetPos();

			if (specifiedPos) {
				Coordinate x = ctx.GetArgument<Coordinate>("x");
				Coordinate y = ctx.GetArgument<Coordinate>("y");
				pos = new Vec2d(x.GetPos(pos.x), y.GetPos(pos.y));
			}
			ctx.Source.execServices.Level.SpawnEntity(entityDescriptor, pos);
			outputConsumer("Spawned new {} at {}".WithArgs(entityDescriptor, pos));
			return 1;
		}
	}
}
