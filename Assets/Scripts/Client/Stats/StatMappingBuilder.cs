using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CSharp;
using Unity.VisualScripting;
using UnityEngine;

public sealed class StatMappingBuilder {
	public DynamicMap<AbstractSerializableStat> dynamicStats { get; private set; }
	public DynamicMap<IStatEffectHandler> dynamicEffectHandlers { get; private set; }
	public IEnumerable<StatActivator> activators { get; private set; }
	public IEnumerable<TooltipNodeData> tooltipNodes { get; private set; }

	public StatMappingBuilder(Func<DynamicMap<AbstractSerializableStat>> dynamicStatSupplier = null) {
		dynamicStats = dynamicStatSupplier?.Invoke() ?? new DynamicMap<AbstractSerializableStat>();
	}

	public StatMappingBuilder AddStats(Func<DynamicMap<AbstractSerializableStat>> dynamicStatSupplier) {
		dynamicStats.AddRange(dynamicStatSupplier.Invoke());
		return this;
	}

	public StatMappingBuilder SetStats(Func<DynamicMap<AbstractSerializableStat>> dynamicStatSupplier) {
		dynamicStats = dynamicStatSupplier.Invoke();
		return this;
	}

	public StatMappingBuilder BindEffectHandlers(Func<DynamicMap<AbstractSerializableStat>, DynamicMap<IStatEffectHandler>> effectHandlerBinder) {
		dynamicEffectHandlers = effectHandlerBinder.Invoke(dynamicStats);
		return this;
	}

	public StatMappingBuilder BindActivators(Func<DynamicMap<IStatEffectHandler>, IEnumerable<StatActivator>> activatorBinder) {
		activators = activatorBinder.Invoke(dynamicEffectHandlers);
		return this;
	}

	public StatMappingBuilder WithTooltipNodes(Func<DynamicMap<AbstractSerializableStat>, IEnumerable<TooltipNodeData>> nodesSupplier) {
		this.tooltipNodes = nodesSupplier.Invoke(dynamicStats);
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
