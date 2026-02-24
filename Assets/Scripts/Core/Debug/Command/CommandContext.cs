using System.Collections.Generic;

namespace SoulboundBackend.Core.Debug.Commands {
	public class CommandContext {
		private readonly Dictionary<string, object> arguments  = new();

		public T GetArgument<T>(string key) => (T)arguments[key];
		public bool TryGet<T>(string key, out T value) {
			bool found = arguments.TryGetValue(key, out var boxed);
			value = (T)boxed;
			return found;
		}

		public void SetArgument<T>(string key, T value) => arguments[key] = value;
	}
}
