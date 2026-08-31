namespace SoulboundEngine.UnityClient.Debug.Command {
	using Brigadier.NET;
	using SoulboundEngine.Command;
	using System;

#nullable enable

	public sealed class ClientLevelCommandProvider : ICommandProvider<ClientCommandContext> {
		void ICommandProvider<ClientCommandContext>.RegisterCommands(CommandDispatcher<ClientCommandContext> dispatcher, Action<string> outputConsumer) {
			new SetblockCommand<ClientCommandContext>().Supply(dispatcher.Register, outputConsumer);
			new TeleportCommand<ClientCommandContext>().Supply(dispatcher.Register, outputConsumer);
			new SpawnCommand<ClientCommandContext>().Supply(dispatcher.Register, outputConsumer);
			new GiveCommand<ClientCommandContext>().Supply(dispatcher.Register, outputConsumer);
		}

	}
}
