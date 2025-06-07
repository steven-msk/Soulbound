using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Item")]
public class Item : ScriptableObject, ISerializable {
	[SerializeField] private string itemName;
	[SerializeField] private string id;
	[SerializeField] private Sprite icon;
	[SerializeField] private GameObject worldPrefab;
	[SerializeField] private int maxStackSize;
	[SerializeField] private List<ItemTag> tags;

	public string Name => itemName;
	public Sprite Icon => icon;
	public GameObject WorldPrefab => worldPrefab;
	public int MaxStackSize => maxStackSize;
	public bool IsStackable => maxStackSize > 1;
	public string ID => id;
	public IReadOnlyList<ItemTag> Tags => tags;
	public bool HasTag(ItemTag tag) => tags.Contains(tag);
	public bool HasAnyTag(params ItemTag[] checkTags) => checkTags.Any(tag => tags.Contains(tag));
}
