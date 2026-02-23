using SoulboundBackend.Client.UI;
using System;

#nullable enable

namespace SoulboundBackend.Client.SettingSystem {
	public abstract class AbstractSettingEntry {
		public readonly string displayName;
		public readonly string id;
		public abstract object boxedDefaultValue { get; }
		public abstract object boxedValue { get; }
		public abstract Type valueType { get; }

		protected AbstractSettingEntry(string name, string id) {
			this.displayName = name;
			this.id = id;
		}

		public override string ToString() {
			return $"{displayName}={boxedValue}";
		}
	}
}
