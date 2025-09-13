using System;
using System.Collections.Generic;
using System.Linq;

public sealed class StatMappingBuilder {
	public DynamicMap<AbstractSerializableStat> dynamicStats { get; private set; } = new();
	public DynamicMap<IStatEffectHandler> dynamicEffectHandlers { get; private set; } = new();
	public List<StatActivator> activators { get; private set; } = new();
	public List<TooltipNodeData> tooltipNodes { get; private set; } = new();

	public StatMappingBuilder(Func<DynamicMap<AbstractSerializableStat>> dynamicStatSupplier = null) {
		dynamicStats = dynamicStatSupplier?.Invoke() ?? new DynamicMap<AbstractSerializableStat>();
	}

	public StatMappingBuilder AddStats(Func<DynamicMap<AbstractSerializableStat>> dynamicStatSupplier) {
		dynamicStats.AddRange(dynamicStatSupplier.Invoke());
		return this;
	}

	public StatMappingBuilder SetStats(Func<DynamicMap<AbstractSerializableStat>> dynamicStatSupplier) {
		dynamicStats.AddRange(dynamicStatSupplier.Invoke());
		return this;
	}

	public StatMappingBuilder BindEffectHandlers(Func<DynamicMap<AbstractSerializableStat>, DynamicMap<IStatEffectHandler>> effectHandlerBinder) {
		dynamicEffectHandlers.AddRange(effectHandlerBinder.Invoke(dynamicStats));
		return this;
	}

	public StatMappingBuilder BindActivators(Func<DynamicMap<IStatEffectHandler>, IEnumerable<StatActivator>> activatorBinder) {
		activators.AddRange(activatorBinder.Invoke(dynamicEffectHandlers));
		return this;
	}

	public StatMappingBuilder BindActivator(Func<DynamicMap<IStatEffectHandler>, StatActivator> activatorBinder) {
		activators.Add(activatorBinder.Invoke(dynamicEffectHandlers));
		return this;
	}

	public StatMappingBuilder WithTooltipNodes(Func<DynamicMap<AbstractSerializableStat>, IEnumerable<TooltipNodeData>> nodesSupplier) {
		this.tooltipNodes.AddRange(nodesSupplier.Invoke(dynamicStats));
		return this;
	}

	public List<StatMapping> ResolveMappings() {
		Dictionary<StatActivator, IEnumerable<AbstractSerializableStat>> statsByActivator = new();
		foreach (var activator in activators) {
			statsByActivator[activator] = activator.effectHandlers.SelectMany(s => s.SuppliedStats());
		}

		// "Reverse" dictionary to return activators by stat
		return statsByActivator
			.SelectMany(pair => pair.Value.Select(stat => (stat, pair.Key)))
			.GroupBy(x => x.stat, x => x.Key)
			.ToDictionary(g => g.Key, g => g.ToList())
			.Select(kvp => new StatMapping(kvp.Key, kvp.Value))
			.ToList();
	}

	public TooltipNodeData[] ResolveTooltipNodes() {
		return this.tooltipNodes.ToArray();
	}
}
