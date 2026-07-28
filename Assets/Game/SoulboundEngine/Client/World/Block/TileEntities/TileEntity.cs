using Newtonsoft.Json.Linq;
using SoulboundEngine.Client.World.Block.State;

#nullable enable

namespace SoulboundEngine.Client.World.Block.Entity {
	using Level = Level.Level;

	public abstract class TileEntity {
		protected Level? level;
		public readonly BlockPos blockPos;
		protected readonly TileEntityType tileEntityType;
		protected readonly BlockState blockState;

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

		public bool HasLevel() => this.level != null;

		public virtual void Write(JObject json) {
		}

		public virtual void Read(JToken json) {
		}
	}
}
