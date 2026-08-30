namespace SoulboundEngine.UnityClient.Debug.Command {
	using Brigadier.NET;
	using System;

	public interface ICommandProvider {
		void RegisterCommands(CommandDispatcher<RuntimeCommandSource> dispatcher, Action<string> outputConsumer);
	}
}
