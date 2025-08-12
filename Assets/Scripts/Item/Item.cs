using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

#nullable enable

public abstract class Item {
	public static readonly int universalMaxStack = 999_999;

	// FIXME: mismatched README version - item dev notes are obsolete

	public abstract string name { get; }
	public abstract Sprite icon { get; }
	// POTENTIAL REWORK: item world prefab serialization
	public abstract Func<GameObject> worldPrefabSupplier { get; }
	public abstract int maxStackSize { get; }
	public bool IsStackable => maxStackSize > 1;
	protected abstract Func<Item, AbstractTooltip?> tooltipSupplier { get; }

	public AbstractTooltip? GetTooltip() => tooltipSupplier.Invoke(this);

	public virtual GameObject FallbackWorldPrefab() {
		GameObject fallback = InstantiateDefaultWorldPrefab(); 
		fallback.GetComponent<SpriteRenderer>().sprite = icon;
		return fallback;
	}

	public static GameObject InstantiateDefaultWorldPrefab() {
		return GameObject.Instantiate(ResourceManager.Get<GameObject, ResourceGroups.Prefabs>("droppedItem"))!;
	}

	//[Obsolete]
	//public AbstractTooltip GetTooltip() {
	//	if (tooltipSupplier != null) {
	//		return tooltipSupplier(this);
	//	}
	//	return GetDefaultTooltip();
	//}

	[Obsolete]
	//protected virtual CompoundTooltip GetDefaultTooltip() => Tooltip.DefaultItem(this);

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
