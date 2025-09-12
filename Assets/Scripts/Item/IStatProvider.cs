using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

[JsonConverter(typeof(StatProviderConverter))]
public interface IStatProvider : IItemCapability {
	// PLANNED REWORK: stat providers trigger other than on hover or select
	public bool applyInstantStatsOnHoverOrSelect { get; }
	public IEnumerable<StatMapping> statMappings { get; }

	public void BeginActivatorContexts(IStatReceiver receiver) {
		GetActivators().ToList().ForEach(a => a.BeginContext(receiver));
	}

	public void EndActivatorContexts(IStatReceiver receiver) {
		GetActivators().ToList().ForEach(a => a.EndContext(receiver));
	}

	protected HashSet<StatActivator> GetActivators() {
		return statMappings.SelectMany(sm => sm.activators).Distinct().ToHashSet();
	}

	public IEnumerable<AbstractSerializableStat> GetStats() {
		return statMappings.Select(sm => sm.stat);
	}

#nullable enable

	private class StatProviderConverter : JsonConverter<IStatProvider> {
		private static readonly Logger logger = Logger.CreateInstance();

		public override IStatProvider? ReadJson(JsonReader reader, Type objectType, IStatProvider? existingValue, bool hasExistingValue, JsonSerializer serializer) {
			int id = Convert.ToInt32(reader.Value);
			IStatProvider? value = null;
			try {
				value = (IStatProvider)Items.ByID(id);
			} catch (Exception e) {
				logger.LogError(null, "Could not deserialize IStatProvider type because the item's id doesnt provide such type. {}", e);
				value = null;
			}
			return value;
		}

		public override void WriteJson(JsonWriter writer, IStatProvider? value, JsonSerializer serializer) {
			serializer.Serialize(writer, (value as Item)?.id);
		}
	}
}