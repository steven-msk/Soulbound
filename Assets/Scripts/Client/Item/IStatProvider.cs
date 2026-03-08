using SoulboundBackend.Client.Stats;
using SoulboundBackend.Common;

using SoulboundBackend.Core.Debug.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SoulboundBackend.Client.ItemSystem {
	[JsonConverter(typeof(StatProviderConverter))]
	[Obsolete]
	public interface IStatProvider {
		public IEnumerable<StatMapping> statMappings { get; }
		public ContextHandle<IStatReceiver> contextHandle { get; }

		protected HashSet<StatActivator> GetActivators() {
			return statMappings.SelectMany(sm => sm.activators).Distinct().ToHashSet();
		}

		public IEnumerable<AbstractValueModifier> GetStats() {
			return statMappings.Select(sm => sm.stat);
		}

#nullable enable

		private class StatProviderConverter : JsonConverter<IStatProvider> {
			public override IStatProvider? ReadJson(JsonReader reader, Type objectType, IStatProvider? existingValue, bool hasExistingValue, JsonSerializer serializer) {
				int id = Convert.ToInt32(reader.Value);
				IStatProvider? value = null;
				try {
					value = (IStatProvider)Items.ByHashedID(id);
				} catch (Exception e) {
					Logger.LogError(null, "Could not deserialize IStatProvider type because the item's id doesnt provide such type. {}", e);
					value = null;
				}
				return value;
			}

			public override void WriteJson(JsonWriter writer, IStatProvider? value, JsonSerializer serializer) {
				serializer.Serialize(writer, (value as Item)?.hashedID);
			}
		}
	}
}
