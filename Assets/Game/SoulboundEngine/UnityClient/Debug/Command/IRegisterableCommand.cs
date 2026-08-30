namespace SoulboundEngine.UnityClient.Debug.Command {
	using Brigadier.NET.Builder;
	using Brigadier.NET.Tree;
	using System;

	public interface IRegisterableCommand {
		void Supply(Func<LiteralArgumentBuilder<RuntimeCommandSource>, LiteralCommandNode<RuntimeCommandSource>> supplier, Action<string> outputConsumer);
	}
}
