using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Item")]
public class Item : ScriptableObject, ISerializable {
	[SerializeField] protected string itemName;
	public virtual string Name => itemName;

	[SerializeField] protected string id;
	public virtual string ID => id;

	// TODO: icon preview in inspector
	[SerializeField] protected Sprite icon;
	public virtual Sprite Icon => icon;

	[SerializeField] protected GameObject worldPrefab;
	public virtual GameObject WorldPrefab => worldPrefab;
	
	[SerializeField] protected int maxStackSize;
	public virtual int MaxStackSize => maxStackSize;
	public virtual bool IsStackable => maxStackSize > 1;

	[SerializeField][CanBeNull] protected TooltipSerializer customTooltipSerializer;
	public TooltipSerializer TooltipSerializer => customTooltipSerializer;

	[SerializeField][CanBeNull] protected string loreTextTooltip;
	public virtual string LoreText => loreTextTooltip;

	[SerializeField][CanBeNull] protected string infoTextTooltip;
	public virtual string InfoText => infoTextTooltip;

	public virtual AbstractTooltip GetTooltip() {
		if (customTooltipSerializer != null) {
			return customTooltipSerializer.GetDeserializer(this).Generate();
		}
		return GetDefaultTooltip();
	}

	protected virtual CompoundTooltip GetDefaultTooltip() => Tooltip.DefaultItem(this);
}
