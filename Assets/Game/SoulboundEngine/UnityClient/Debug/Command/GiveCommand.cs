namespace SoulboundEngine.UnityClient.Debug.Command {
	using Brigadier.NET;
	using Brigadier.NET.Builder;
	using Brigadier.NET.Context;
	using Brigadier.NET.Tree;
	using SoulboundEngine.Common;
	using SoulboundEngine.Item;
	using System;

	public record GiveCommand : IRegisterableCommand {
		public void Supply(Func<LiteralArgumentBuilder<RuntimeCommandSource>, LiteralCommandNode<RuntimeCommandSource>> supplier, Action<string> outputConsumer) {
			supplier(LiteralArgumentBuilder<RuntimeCommandSource>.LiteralArgument("give")
				.Then(c => c.Argument("item", new ItemArgumentType())
					.Executes(ctx => this.GiveItem(1, ctx, outputConsumer))

					.Then(c => c.Argument("count", Arguments.Integer(min: 1))
						.Executes(ctx => this.GiveItem(ctx.GetArgument<int>("count"), ctx, outputConsumer))
					)
				)
			);
		}

		public int GiveItem(int count, CommandContext<RuntimeCommandSource> ctx, Action<string> outputConsumer) {
			Item item = ctx.GetArgument<Item>("item");

			int fullStacks = count / item.GetMaxCount();
			int remainder = count % item.GetMaxCount();

			int stackCount = fullStacks + (remainder > 0 ? 1 : 0);
			ItemStack[] stacks = new ItemStack[stackCount];
			int i;
			for (i = 0; i < fullStacks; i++) {
				stacks[i] = item.GetDefaultStack(item.GetMaxCount());
			}
			if (remainder > 0) stacks[i] = item.GetDefaultStack(remainder);

			for (int j = 0; j < stacks.Length; j++) {
				ctx.Source.execServices.Player.TryAddItemStack(stacks[j]);
			}
			outputConsumer("Gave {} {}".WithArgs(count, item));
			return 1;
		}
	}
}
