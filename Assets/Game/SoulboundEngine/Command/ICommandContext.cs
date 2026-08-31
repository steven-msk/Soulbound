namespace SoulboundEngine.Command {
	using SoulboundEngine.World.Level;
	using System;

	public interface ICommandContext {
		void Run(Action<Level> action);

		T Get<T>(Func<Level, T> function);
	}
}
