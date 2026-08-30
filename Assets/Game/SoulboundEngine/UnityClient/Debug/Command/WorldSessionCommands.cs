namespace SoulboundEngine.UnityClient.Debug.Command {
	using Brigadier.NET;
	using System;

#nullable enable

	public sealed class WorldSessionCommands : ICommandProvider {
		void ICommandProvider.RegisterCommands(CommandDispatcher<RuntimeCommandSource> dispatcher, Action<string> outputConsumer) {
			new SetblockCommand().Supply(dispatcher.Register, outputConsumer);
			new TeleportCommand().Supply(dispatcher.Register, outputConsumer);
			new SpawnCommand().Supply(dispatcher.Register, outputConsumer);
			new GiveCommand().Supply(dispatcher.Register, outputConsumer);
		}

	}
}
