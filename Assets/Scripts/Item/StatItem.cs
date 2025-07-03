using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class StatItem : Item, IStatProvider {
	public abstract bool ApplyInstantStatsAutomatically { get; }

	public abstract List<SerializableStat> InstantStats { get; }

	public abstract List<BufferedStat> BufferedStats { get; }

	public abstract string BufferedInterpolationSource { get; }

	protected override CompoundTooltip GetDefaultTooltip() {
		return base.GetDefaultTooltip().Concat(Tooltip.Stats(InstantStats), Tooltip.InterpolatedStats(BufferedInterpolationSource, BufferedStats)); 
	}

	private void OnValidate() {
		IEnumerable<SerializedStatReference> references = InstantStats.Select(stat => stat.SerializedReference);
		HashSet<SerializedStatReference> referenceInstances = references.ToHashSet();
		foreach (var reference in referenceInstances) {
			IEnumerable<SerializableStat> mapped = InstantStats.Where(stat => stat.SerializedReference == reference);
			if (mapped.Count() > 1) {
				string duplicateEntries = string.Join(", ", mapped.Select(stat => $"{stat.RawValue} {stat.SerializedReference}"));
				Debug.LogWarning($"Duplicate instant stat entries: [{duplicateEntries}] @ {this}");
			}
		}

		foreach (var stat in InstantStats) {
			IStatTypeImpl statType = stat.SerializedReference.ToStatType(); 

			Type expectedType = statType.ValueType; 
			Type type = stat.ValueType.ToInternalType();
			if (type != expectedType) {
				Debug.LogError($"Invalid stat value '{stat.RawValue}' of type {type.Name} for stat entry of type {expectedType.Name}");
			}

			if (!statType.ValidApplications.Contains(stat.ApplicationType)) {
				string accepted = string.Join(", ", statType.ValidApplications);
				Debug.LogWarning($"Mismatched stat value application {stat.ApplicationType} for entry that accepts [{accepted}]: {stat.SerializedReference} @ {this}"); 
			}
		}
	}
}