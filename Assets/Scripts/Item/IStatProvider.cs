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
	public List<AbstractSerializableStat> stats { get; }

	// FEATUREIMPL (WIP): buffered stats - NOT TESTED

	public virtual void ApplyStats(IStatSource source) => source.ApplyProvider(this);

	public virtual void RevokeStats(IStatSource source) => source.RevokeProvider(this);

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