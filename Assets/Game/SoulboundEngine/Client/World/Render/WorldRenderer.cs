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

			RectInt lastView = ToRect(lastPivot);
			this.lastPivot = currentPivot;
			RectInt currentView = ToRect(currentPivot);

			RectInt.PositionEnumerator pos = lastView.allPositionsWithin;
			while (pos.MoveNext()) {
				if (currentView.Contains(pos.Current)) continue;

				RenderBlock((BlockPos)pos.Current, Blocks.air.defaultState);
			}

			pos = currentView.allPositionsWithin;
			while (pos.MoveNext()) {
				BlockPos blockPos = (BlockPos)pos.Current;
				if (!Level.IsInBounds(blockPos) || lastView.Contains(pos.Current)) {
					continue;
				}

				BlockState? blockState = blockStateSupplier(blockPos);
				RenderBlock(blockPos, blockState);
			}
		}

		private void RenderBlock(BlockPos blockPos, BlockState blockState) {
			BlockRenderData renderData = blockState.block.GetRenderData(blockState);
			BlockRenderModel model = modelResolver.ResolveModel(renderData);
			blockRenderer.Render(model, blockPos);
		}

		public void UpdateModel(BlockPos blockPos, BlockState? blockState) {
			blockState ??= Blocks.air.defaultState;
			RenderBlock(blockPos, IsInRenderView(blockPos)
				? blockState
				: Blocks.air.defaultState
			);
		}

		private RectInt ToRect(Vector2Int pivot) {
			return new(
				Mathf.FloorToInt(pivot.x) + renderView.x,
				Mathf.FloorToInt(pivot.y) + renderView.y,
				renderView.width,
				renderView.height
			);
		}

		public bool IsInRenderView(BlockPos blockPos) {
			return ToRect(lastPivot).Contains((Vector2Int)blockPos);
		}
	}
}
