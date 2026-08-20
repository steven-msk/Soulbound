using SoulboundEngine.Registry;
using System;
using System.Collections.Generic;

namespace SoulboundEngine.Client.UI.UXMLBindings {
	public static class UXMLSchema {
		private static readonly Dictionary<Identifier, Type> types = new();
		private static bool frozen;

		public static void Register(Identifier id, Type type) {
			if (frozen) throw new InvalidOperationException("Schema already frozen.");
			types.Add(id, type); // throw on duplicate
		}

		public static void Freeze() => frozen = true;

		public static Type Resolve(Identifier id) {
			return types.TryGetValue(id, out var t) ? t : throw new UXMLBindingException($"Unknown identifier '{id}'.");
		}
	}
}
