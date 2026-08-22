using Brigadier.NET;

namespace SoulboundEngine.UnityClient.Debug.Commands {
	public interface ICommandProvider {
		void RegisterCommands(CommandDispatcher<RuntimeCommandSource> dispatcher);
	}
}
