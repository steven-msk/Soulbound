using UnityEngine;
using UnityEngine.Tilemaps;

public class Tiles : IResourceModule {
	public static Tile air => GetTile<Tile>("air");
	public static RuleTile grass => GetTile<RuleTile>("grass");
	public static Tile stone => GetTile<Tile>("stone");
	public static Tile dirt => GetTile<Tile>("dirt");

	private static TTile GetTile<TTile>(string name) where TTile : TileBase {
		return (TTile)IResourceModule.Resource<TileBase, ResourceGroups.Tiles>(name);
	}
}
