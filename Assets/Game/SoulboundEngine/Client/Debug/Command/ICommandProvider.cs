using Brigadier.NET;

namespace SoulboundEngine.Client.Debug.Commands {
	public interface ICommandProvider {
		void RegisterCommands(CommandDispatcher<RuntimeCommandSource> dispatcher);
	}
}
