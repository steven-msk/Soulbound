namespace SoulboundEngine.Command {
	using Brigadier.NET.Builder;
	using Brigadier.NET.Tree;
	using System;

	public abstract class RegisterableCommand<TContext> where TContext : ICommandContext {
		public abstract void Supply(Func<LiteralArgumentBuilder<TContext>, LiteralCommandNode<TContext>> supplier, Action<string> outputConsumer);
	}
}
