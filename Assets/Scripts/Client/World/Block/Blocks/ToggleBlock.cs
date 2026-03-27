using SoulboundBackend.Client.Debug.Logging;
using SoulboundBackend.Client.Interaction;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.World.BlockSystem.States;
using SoulboundBackend.Core.Assets;
using SoulboundBackend.World.BlockSystem.Render;
using System.Collections.Generic;

namespace SoulboundBackend.Client.World.BlockSystem {
	public sealed class ToggleBlock : Block, IBlockInteractionListener {
		public BlockState on { get; private set; }
		public BlockState off { get; private set; }
		public override string name { get; init; } = "Toggle Block";
		public override int minBreakLevel { get; init; } = 0;

		public ToggleBlock() : base("toggleBlock") { }

		public void OnInteract(in BlockInteraction ctx) {
			bool isOn = ctx.blockState.Get<bool>("on");
			ctx.level.SetBlockState(ctx.blockPos, isOn ? off : on);
			Logger.LogInfo("block at {} is now {}", ctx.blockPos, isOn ? "off" : "on");
		}

		public bool CanInteract(in BlockInteraction ctx) => true;

		public bool ValidateTrigger(InteractionTrigger trigger) {
			return trigger == InteractionTrigger.RightClick;
		}

		protected override BlockState GetDefaultState(IBlockStateRegisterer registerer, BlockPropertyEntries properties) {
			return on;
		}

		protected override void CreateStates(IBlockStateRegisterer registerer, BlockPropertyEntries properties) {
			on = registerer.AddWithProperties(properties.With("on", true));
			off = registerer.AddWithProperties(properties.With("on", false));
		}

		public override IEnumerable<ItemStack> GetDrops(BlockState blockState, BreakSource source) {
			yield break;
		}

		public override BlockRenderData GetRenderData(BlockState blockState) {
			return new BlockRenderData(blockState.Get<bool>("on")
				? new AssetKey("ToggleOnTile")
				: new AssetKey("ToggleOffTile")
			);
		}
	}
}
