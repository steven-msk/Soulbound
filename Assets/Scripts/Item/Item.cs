using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

#nullable enable

[CreateAssetMenu(menuName = "Items/Item")]
public class Item : ScriptableObject, ISerializable {
	[SerializeField] protected string itemName;
	public virtual string Name => itemName;

	[SerializeField] protected string id;
	public virtual string ID => id;

	// TODO: icon preview in inspector
	[SerializeField] protected Sprite icon;
	public virtual Sprite Icon => icon;

	// POTENTIAL REWORK: item world prefab serialization
	[SerializeField] protected GameObject worldPrefab;
	public virtual GameObject WorldPrefab => worldPrefab;
	
	[SerializeField] protected int maxStackSize;
	public virtual int MaxStackSize => maxStackSize;
	public virtual bool IsStackable => maxStackSize > 1;

	[SerializeField]protected TooltipSerializer? customTooltipSerializer;
	public TooltipSerializer? TooltipSerializer => customTooltipSerializer;

	[SerializeField] protected string? loreTextTooltip;
	public virtual string? LoreText => loreTextTooltip;

	[SerializeField] protected string? infoTextTooltip;
	public virtual string? InfoText => infoTextTooltip;

	public virtual AbstractTooltip GetTooltip() {
		if (customTooltipSerializer != null) {
			return customTooltipSerializer.GetDeserializer(this).Generate();
		}
		return GetDefaultTooltip();
	}

	protected virtual CompoundTooltip GetDefaultTooltip() => Tooltip.DefaultItem(this);

	public bool HasCapability<T>() where T : IItemCapability {
		return typeof(T).IsAssignableFrom(this.GetType());
    }

	public bool HasCapability(Type type) {
		return type.IsAssignableFrom(this.GetType());
    }
}
