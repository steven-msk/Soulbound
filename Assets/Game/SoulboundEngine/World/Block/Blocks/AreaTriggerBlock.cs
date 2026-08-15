using SoulboundEngine.Common;
using SoulboundEngine.Core.States;
using SoulboundEngine.World.Block.Entity;
using SoulboundEngine.World.Block.State;
using SoulboundEngine.World.Player;
using Logger = SoulboundEngine.Client.Debug.Logging.Logger;

namespace SoulboundEngine.World.Block {
	using Level = Level.Level;

	[PROTOTYPICAL]
	public sealed class AreaTriggerBlock : Block, ITileEntityProvider {
		public static readonly Property<bool> inArea = BoolProperty.Of("inArea");

		public AreaTriggerBlock(AbstractBlock.Settings settings) 
			: base(settings) {
			this.SetDefaultState(this.DefaultState.With(inArea, false));
		}

		protected override void AppendProperties(StateManager<Block, BlockState>.Builder builder) {
			builder.Add(inArea);
		}

		private void OnAreaEnter(Level level, BlockPos selfPos, PlayerEntity player) {
			Logger.LogInfo("onAreaEnter");
			level.SetBlockState(selfPos, this.DefaultState.With(inArea, true));
		}

		private void OnAreaExit(Level level, BlockPos selfPos, PlayerEntity player) {
			Logger.LogInfo("onAreaExit");
			level.SetBlockState(selfPos, this.DefaultState.With(inArea, false));
		}

		public TileEntity CreateTileEntity(BlockPos pos, BlockState state) {
			ObjectTileEntity tileEntity = ObjectTileEntity.Create(pos, state);

			tileEntity.onTriggerEnter += player => this.OnAreaEnter(tileEntity.GetLevel(), tileEntity.GetBlockPos(), player);
			tileEntity.onTriggerExit += player => this.OnAreaExit(tileEntity.GetLevel(), tileEntity.GetBlockPos(), player);

			return tileEntity;
		}
	}
}
