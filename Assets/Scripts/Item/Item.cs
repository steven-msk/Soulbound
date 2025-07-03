using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Item")]
public class Item : ScriptableObject, ISerializable {
	[SerializeField] protected string itemName;
	public string Name => itemName;

	[SerializeField] protected string id;
	public string ID => id;

	[SerializeField] protected Sprite icon;
	public Sprite Icon => icon;

	[SerializeField] protected GameObject worldPrefab;
	public GameObject WorldPrefab => worldPrefab;
	
	[SerializeField] protected int maxStackSize;
	public int MaxStackSize => maxStackSize;
	public bool IsStackable => maxStackSize > 1;

	[SerializeField][CanBeNull] protected TooltipSerializer customTooltipSerializer;
	public TooltipSerializer TooltipSerializer => customTooltipSerializer;

	[SerializeField][CanBeNull] protected string loreTextTooltip;
	public string LoreText => loreTextTooltip;

	[SerializeField][CanBeNull] protected string infoTextTooltip;
	public string InfoText => infoTextTooltip;

	public virtual AbstractTooltip GetTooltip() {
		if (customTooltipSerializer != null) {
			return customTooltipSerializer.GetDeserializer(this).Generate();
		}
		return GetDefaultTooltip();
	}

	protected virtual CompoundTooltip GetDefaultTooltip() => Tooltip.DefaultItem(this);
}
