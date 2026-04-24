namespace SoulboundEngine.Core.GameState {
	public static class GameState {
		public const int UNINITIALIZED = 1 << 0;	// 1
		public const int BOOTSTRAPPING = 1 << 1;	// 2
		public const int INITIALIZED   = 1 << 2;	// 4
		public const int LAUNCHING	   = 1 << 3;	// 8
		public const int RUNNING	   = 1 << 4;	// 16
		public const int SHUTDOWN	   = 1 << 5;	// 32
		public const int TERMINATED	   = 1 << 6;	// 64
	}
}
