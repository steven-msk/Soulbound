using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Block")]
public class Block : ScriptableObject, ISerializable {
	[SerializeField] private string blockName; 
	public new string name => blockName;

	[SerializeField] private string blockID;
	public string ID => blockID;

	[SerializeField] private TileBase tileReference;
	public TileBase TileReference => tileReference;
}
