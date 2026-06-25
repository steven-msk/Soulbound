namespace SoulboundEngine.Client.World.Block.TileEntity {
	public abstract class TileEntityType {
	}

	public class TileEntityType<TE> : TileEntityType where TE : TileEntity { 
	}
}
