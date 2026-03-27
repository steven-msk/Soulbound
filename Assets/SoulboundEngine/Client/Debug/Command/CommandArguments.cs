using System.Collections.Generic;

namespace SoulboundEngine.Client.Debug.Commands {
	public class CommandArguments {
		private readonly Dictionary<string, object> arguments  = new();

		public T Get<T>(string key) => (T)arguments[key];
		public bool TryGet<T>(string key, out T value) {
			bool found = arguments.TryGetValue(key, out var boxed);
			value = found ? (T)boxed : default;
			return found;
		}

		public void Set<T>(string key, T value) => arguments[key] = value;
	}
}
