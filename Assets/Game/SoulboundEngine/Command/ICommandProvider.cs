namespace SoulboundEngine.Command {
	using Brigadier.NET;
	using System;

	public interface ICommandProvider<TContext> where TContext : ICommandContext {
		void RegisterCommands(CommandDispatcher<TContext> dispatcher, Action<string> outputConsumer);
	}
}
