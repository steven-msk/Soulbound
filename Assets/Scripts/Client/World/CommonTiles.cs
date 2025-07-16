using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class CommonTiles {
	public static Tile air = Registry.Get<Tile>("air");
	public static RuleTile grass = Registry.Get<RuleTile>("grass");
	public static Tile stone = Registry.Get<Tile>("stone");
}
