namespace SoulboundEngine.UnityClient.Debug.Command {
	using SoulboundEngine.Command;
	using SoulboundEngine.World.Level;
	using System;

#nullable enable

	public record ClientCommandContext(SoulboundUnityClient client) : ICommandContext {
		private Level? level;

		public T Get<T>(Func<Level, T> function) {
			return function(this.level ?? throw new NotSupportedException("Tried to get data with a null level!"));
		}

		public void Run(Action<Level> action) {
			if (this.level != null) {
				action(this.level);
			}
		}

		public void SetLevel(Level? level) {
			this.level = level;
		}
	}
}
