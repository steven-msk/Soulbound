using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Level {
	private Grid grid;

	private PlayerController player;
	public PlayerController Player => player;

	private Tilemap tilemap;
	public Tilemap WorldTilemap => tilemap;


	public Level(PlayerController player, Tilemap tilemap) {
		this.player = player;
		this.tilemap = tilemap;
	}

	public void Bootstrap() {

		// temporary terrain
		int startY = -200;
		int width = 1000;
		int height = 100;
		TileBase[,] tiles = new TileBase[height, width];
		for (int x = 0; x < width; x++) {
			float noise = Mathf.PerlinNoise1D(x * 0.05f);
			int terrainHeight = Mathf.FloorToInt(noise * 20f);

			for (int y = 0; y < height; y++) {
				tiles[y, x] = y <= terrainHeight
					? Registry.Get<RuleTile>("grass")
					: Registry.Get<Tile>("air");
			}
		}
		tilemap.SetTilesBlock(new BoundsInt(0, 0, 0, width, height, 1), tiles.Cast<TileBase>().ToArray());
	}
}
