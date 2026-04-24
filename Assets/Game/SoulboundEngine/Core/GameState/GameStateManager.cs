using System;

namespace SoulboundEngine.Core.GameState {
	internal static class GameStateManager {
		private static int currentState = GameState.UNINITIALIZED;

		public static void SetBootstrapping() => SwitchOrThrow(GameState.BOOTSTRAPPING);
		public static void SetInitialized() => SwitchOrThrow(GameState.INITIALIZED);
		public static void SetLaunching() => SwitchOrThrow(GameState.LAUNCHING);
		public static void SetRunning() => SwitchOrThrow(GameState.RUNNING);
		public static void SetShutdown() => SwitchOrThrow(GameState.SHUTDOWN);
		public static void SetTerminated() => SwitchOrThrow(GameState.TERMINATED);

		private static void SwitchOrThrow(int targetFlag) {
			int flag = currentState;
			flag <<= 1;

			if (flag != targetFlag) {
				throw new InvalidOperationException($"Cannot switch game state from {currentState} to {targetFlag}");
			}

			currentState = targetFlag;
		}

		public static int GetCurrent() => currentState;
	}
}
