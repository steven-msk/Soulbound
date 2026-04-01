using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Core.Registry;
using System;
using System.Collections.Generic;

namespace SoulboundEngine.Core {
	public static class Registry<T> {
		private static readonly Dictionary<Identifier, T> registry = new();

		public static TSub Add<TSub>(TSub value) where TSub : T, IIdentifierProvider {
			return Add(value.GetIdentifier(), value);
		}

		public static TSub Add<TSub>(Identifier identifier, TSub value) where TSub : T {
			if (registry.ContainsKey(identifier) || IdentifierRegistry.ContainsIdentifier(identifier)) {
				Logger.LogError("Identifier already exists: {}", identifier);
				return value;
			}
			registry.Add(identifier, value);
			IdentifierRegistry.AddIdentifier(identifier);
			return value;
		}

		public static T Get(Identifier identifier) {
			if (!registry.TryGetValue(identifier, out T value)) {
				Logger.LogFatal(new ArgumentException($"Could not find registry element with identifier '{identifier}'"));
				return default;
			}
			return value;
		}

		public static bool TryGet(Identifier identifier, out T value) {
			return registry.TryGetValue(identifier, out value);
		}

		public static void Remove(Identifier identifier) {
			registry.Remove(identifier);
			IdentifierRegistry.RemoveIdentifier(identifier);
		}

		public static IEnumerable<T> GetAll() => registry.Values;

	}

	internal static class IdentifierRegistry {
		private static readonly HashSet<Identifier> allIdentifiers = new();

		public static bool ContainsIdentifier(Identifier identifier) {
			return allIdentifiers.Contains(identifier);
		}

		public static void AddIdentifier(Identifier identifier) => allIdentifiers.Add(identifier);

		public static void RemoveIdentifier(Identifier identifier) => allIdentifiers.Remove(identifier);
	}
}
