using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Block")]
public class Block : ScriptableObject {
	[SerializeField] private string blockName;
	public string BlockName => blockName;

	[SerializeField] private TileBase tileReference;
	public TileBase TileReference => tileReference;
}
