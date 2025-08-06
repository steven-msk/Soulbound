using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class StatItemDefinition : ItemDefinition, IStatProvider {
    public abstract bool applyInstantStatsAutomatically { get; }
    public List<SerializableStat> instantStats { get; }
    public List<BufferedStat> bufferedStats { get; }
    public virtual string bufferedInterpolationSource { get; }

    public StatItemDefinition(string name, Sprite icon, Func<GameObject> worldPrefabSupplier, int maxStackSize, Func<Item, AbstractTooltip> tooltipSupplier,
            List<SerializableStat> instantStats, List<BufferedStat> bufferedStats, string interpolationSource)
        : base(name, icon, worldPrefabSupplier, maxStackSize, tooltipSupplier) {
        this.instantStats = instantStats;
        this.bufferedStats = bufferedStats;
        this.bufferedInterpolationSource = interpolationSource;
    }

    public virtual void ValidateStats() {
        IEnumerable<SerializedStatReference> references = instantStats.Select(stat => stat.SerializedReference);
        HashSet<SerializedStatReference> referenceInstances = references.ToHashSet();
        foreach (var reference in referenceInstances) {
            IEnumerable<SerializableStat> mapped = instantStats.Where(stat => stat.SerializedReference == reference);
            if (mapped.Count() > 1) {
                string duplicateEntries = string.Join(", ", mapped.Select(stat => $"{stat.RawValue} {stat.SerializedReference}"));
                Debug.LogWarning($"Duplicate instant stat entries: [{duplicateEntries}] @ {this}");
            }
        }

        void Validate(IEnumerable<SerializableStat> stats) {
            foreach (var stat in stats) {
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
                if (string.IsNullOrEmpty(stat.RawValue)) {
                    Debug.LogError($"No value available for {stat} @ {this}");
                }
            }
        }
        void ValidateBufferedTriggers(Func<BufferedStat, string> emptyTriggerMessage) {
            foreach (var stat in bufferedStats) {
                if (stat.ApplyBufferedTrigger == null) {
                    Debug.LogError(emptyTriggerMessage.Invoke(stat));
                }
            }
        }
        Validate(instantStats);
        Validate(bufferedStats);
        ValidateBufferedTriggers(stat => $"No apply condition selected for stat {stat.SerializedReference} @ {this}");
        ValidateBufferedTriggers(stat => $"No revoke condition selected for stat {stat.SerializedReference} @ {this}");
        bufferedStats.ForEach(stat => {
            stat.ApplyBufferedTrigger.ValidateExecution(stat, this, true);
            stat.RevokeBufferedTrigger.ValidateExecution(stat, this, true);
        });
    }
}