using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Client.World.BlockSystem.Render;
using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Client.World.LevelDomain;
using System;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.World.Render {
	public sealed class WorldRenderer {
		private readonly BlockRenderer blockRenderer;
		private readonly BlockModelResolver modelResolver;
		private Vector2Int lastPivot;
		private readonly RectInt renderView;

		public WorldRenderer(
				RectInt renderView,
				BlockRenderer blockRenderer,
				BlockModelResolver modelResolver
			) {
			this.renderView = renderView;
			this.blockRenderer = blockRenderer;
			this.modelResolver = modelResolver;
		}

		public void RenderView(Vector2 pivot, Func<BlockPos, BlockState> blockStateSupplier) {
			Vector2Int currentPivot = Vector2Int.FloorToInt(pivot);
			if (this.lastPivot == currentPivot) return;

			RectInt lastView = this.ToRect(this.lastPivot);
			this.lastPivot = currentPivot;
			RectInt currentView = this.ToRect(currentPivot);

			RectInt.PositionEnumerator pos = lastView.allPositionsWithin;
			while (pos.MoveNext()) {
				if (currentView.Contains(pos.Current)) continue;

				this.RenderBlock((BlockPos)pos.Current, Blocks.AIR.DefaultState);
			}

			pos = currentView.allPositionsWithin;
			while (pos.MoveNext()) {
				BlockPos blockPos = (BlockPos)pos.Current;
				if (!Level.IsInBounds(blockPos) || lastView.Contains(pos.Current)) {
					continue;
				}

				BlockState? blockState = blockStateSupplier(blockPos);
				this.RenderBlock(blockPos, blockState);
			}
		}

		private void RenderBlock(BlockPos blockPos, BlockState blockState) {
			BlockRenderData renderData = blockState.block.GetRenderData(blockState);
			BlockRenderModel model = this.modelResolver.ResolveModel(renderData);
			this.blockRenderer.Render(model, blockPos);
		}

		public void UpdateModel(BlockPos blockPos, BlockState? blockState) {
			blockState ??= Blocks.AIR.DefaultState;
			this.RenderBlock(blockPos, this.IsInRenderView(blockPos)
				? blockState
				: Blocks.AIR.DefaultState
			);
		}

		private RectInt ToRect(Vector2Int pivot) {
			return new(
				Mathf.FloorToInt(pivot.x) + this.renderView.x,
				Mathf.FloorToInt(pivot.y) + this.renderView.y,
				this.renderView.width,
				this.renderView.height
			);
		}

		public bool IsInRenderView(BlockPos blockPos) {
			return this.ToRect(this.lastPivot).Contains((Vector2Int)blockPos);
		}
	}
}
