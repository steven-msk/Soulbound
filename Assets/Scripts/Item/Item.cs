using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using log4net.Core;
using Unity.VisualScripting;
using UnityEngine;

#nullable enable

public abstract partial class Item {
	private static readonly Logger logger = Logger.CreateInstance();
	public static readonly int universalMaxStack = 999_999;

	// FIXME: mismatched README version - item dev notes are obsolete

	public abstract string name { get; }
	public abstract Sprite icon { get; }
	// POTENTIAL REWORK: item world prefab serialization
	public abstract Func<GameObject> worldPrefabSupplier { get; }
	public abstract int maxStackSize { get; }
	public bool IsStackable => maxStackSize > 1;
	protected abstract Func<Item, TooltipData?> tooltipSupplier { get; }
	protected abstract TooltipRenderer.NodeStyleProvider? nodeStyleProvider { get; }

	public virtual GameObject FallbackWorldPrefab() {
		GameObject fallback = InstantiateDefaultWorldPrefab(); 
		fallback.GetComponent<SpriteRenderer>().sprite = icon;
		return fallback;
	}

	public static GameObject InstantiateDefaultWorldPrefab() {
		return GameObject.Instantiate(ResourceManager.Get<GameObject, ResourceGroups.Prefabs>("droppedItem"))!;
	}

	public Tooltip? RenderTooltip(Vector2 position, Transform parent) {
		if (tooltipSupplier == null) {
			logger.LogWarning(null, "No tooltip supplier for item '{}'. This may be intentional.", this.name);
			return null;
		}
		TooltipData tooltipData = tooltipSupplier.Invoke(this) ?? Tooltip.Plain(this.name);
		TooltipRenderer renderer = new(nodeStyleProvider ?? TooltipNodeStylePresets.PresetProvider());
		Tooltip tooltip = new Tooltip(renderer, tooltipData);
		tooltip.Show(position, parent);
		return tooltip;
	}

	public static int CustomMaxStack(int maxStack) => maxStack;

	public bool HasCapability<T>() where T : IItemCapability {
		return typeof(T).IsAssignableFrom(this.GetType());
    }

	public bool HasCapability(Type type) {
		return type.IsAssignableFrom(this.GetType());
    }

    public override string ToString() {
		return name;
    }
}
