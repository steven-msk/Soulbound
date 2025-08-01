using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface IPlaceable : IItemCapability {
	BlockState Place(ItemStack itemStack, BlockPos position);
}
