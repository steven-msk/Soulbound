using Newtonsoft.Json.Linq;
using SoulboundEngine.World.Block.State;

#nullable enable

namespace SoulboundEngine.World.Block.Entity {
	using Level = Level.Level;
	
	public abstract class TileEntity {
		protected Level? level;
		public readonly BlockPos blockPos;
		protected readonly TileEntityType tileEntityType;
		protected BlockState blockState;

		public TileEntity(TileEntityType tileEntityType, BlockPos blockPos, BlockState blockState) {
			this.tileEntityType = tileEntityType;
			this.blockPos = blockPos;
			this.blockState = blockState;
		}

		public virtual void OnDispose() { }

		public TileEntityType GetTileEntityType() => this.tileEntityType;

		public void SetLevel(Level? level) => this.level = level;
		public Level? GetLevel() => this.level;

		public BlockPos GetBlockPos() => this.blockPos;

		public BlockState GetBlockState() => this.blockState;
		public void SetBlockState(BlockState blockState) => this.blockState = blockState;

		public bool HasLevel() => this.level != null;

		public virtual void Write(JObject json) {
		}

		public virtual void Read(JToken json) {
		}

		public bool IsValidBlockState(BlockState blockState) {
			return this.GetTileEntityType().Supports(blockState);
		}

		public override string ToString() => this.GetTileEntityType().ToString();
	}
}
