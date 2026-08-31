namespace SoulboundEngine.Command {
	using Brigadier.NET;
	using Brigadier.NET.Builder;
	using Brigadier.NET.Context;
	using Brigadier.NET.Tree;
	using SoulboundEngine.Common;
	using SoulboundEngine.Item;
	using System;

	public class GiveCommand<TContext> : RegisterableCommand<TContext> where TContext : ICommandContext {
		public override void Supply(Func<LiteralArgumentBuilder<TContext>, LiteralCommandNode<TContext>> supplier, Action<string> outputConsumer) {
			supplier(LiteralArgumentBuilder<TContext>.LiteralArgument("give")
				.Then(c => c.Argument("item", new ItemArgumentType())
					.Executes(ctx => this.GiveItem(1, ctx, outputConsumer))

					.Then(c => c.Argument("count", Arguments.Integer(min: 1))
						.Executes(ctx => this.GiveItem(ctx.GetArgument<int>("count"), ctx, outputConsumer))
					)
				)
			);
		}

		public virtual int GiveItem(int count, CommandContext<TContext> ctx, Action<string> outputConsumer) {
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

			ctx.Source.Run(level => {
				for (int j = 0; j < stacks.Length; j++) {
					ItemStack remainder = level.GetPlayer().Take(stacks[j]);
					if (!remainder.IsEmpty()) {
						level.GetPlayer().DropStack(remainder);
					}
				}
				outputConsumer("Gave {} {}".WithArgs(count, item));
			});
			return 1;
		}
	}
}
