using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Item")]
public class Item : ScriptableObject, ISerializable {
	[SerializeField] private string itemName;
	[SerializeField] private string id;
	[SerializeField] private Sprite icon;
	[SerializeField] private GameObject worldPrefab;
	[SerializeField] private int maxStackSize;

	public string Name => itemName;
	public Sprite Icon => icon;
	public GameObject WorldPrefab => worldPrefab;
	public int MaxStackSize => maxStackSize;
	public bool IsStackable => maxStackSize > 1;
	public string ID => id;
}
