using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface IPlaceable : IItemCapability {
	Block Place(ItemStack itemStack, Vector2Int position, Tilemap tilemap);
}
