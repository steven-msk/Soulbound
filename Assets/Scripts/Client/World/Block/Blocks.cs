using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Blocks {
    public static Block air = FromRegistry("air_block");
    public static Block stone = FromRegistry("stone_block");
    public static Block dirt = FromRegistry("dirt_block");
    public static Block grass = FromRegistry("grass_block");
    public static Block wood = FromRegistry("wood_block");
    public static Block leaf = FromRegistry("leaf_block");

    public static Block FromRegistry(string id) => AssetRegistry.Get<Block>(id);
}
