using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Item")]
public class Item : ScriptableObject, ISerializable {

	// TODO: implement better item abstraction - inherit interface properties instead of directly abstract classes (IConsumable, IStatProvider, etc.)

	[SerializeField] protected string itemName;
	[SerializeField] protected string id;
	[SerializeField] protected Sprite icon;
	[SerializeField] protected GameObject worldPrefab;
	[SerializeField] protected int maxStackSize;
	[SerializeField][CanBeNull] protected AbstractTooltipSerializer customTooltipSerializer;
	[SerializeField][CanBeNull] protected string loreTextTooltip;
	[SerializeField][CanBeNull] protected string infoTextTooltip;

	public string Name => itemName;
	public Sprite Icon => icon;
	public GameObject WorldPrefab => worldPrefab;
	public int MaxStackSize => maxStackSize;
	public bool IsStackable => maxStackSize > 1;
	public string ID => id;
	public AbstractTooltipSerializer TooltipSerializer => customTooltipSerializer;
	public string LoreText => loreTextTooltip;
	public string InfoText => infoTextTooltip;

	public virtual AbstractTooltip GetTooltip() {
		if (customTooltipSerializer != null) {
			return customTooltipSerializer.GetSerializer(this).Generate();
		}
		return GetDefaultTooltip();
	}

	protected virtual AbstractTooltip GetDefaultTooltip() {
		return CompoundTooltip.Of(Tooltip.Title(itemName), Tooltip.Info(infoTextTooltip), Tooltip.Lore(loreTextTooltip));
	}
}
