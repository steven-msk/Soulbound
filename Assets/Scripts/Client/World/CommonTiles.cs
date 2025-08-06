using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class CommonTiles {
	public static Tile air => AssetRegistry.Get<Tile>("air");
	public static RuleTile grass => AssetRegistry.Get<RuleTile>("grass_air_test");
	public static Tile stone => AssetRegistry.Get<Tile>("stone");
	public static Tile dirt => AssetRegistry.Get<Tile>("dirt");
}
