namespace SoulboundEngine.Client.World.BlockSystem.TileEntities {
	public abstract class TileEntityType {
	}

	public class TileEntityType<TE> : TileEntityType where TE : TileEntity { 
	}
}
