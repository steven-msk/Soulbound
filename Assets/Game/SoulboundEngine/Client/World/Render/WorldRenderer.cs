using SoulboundEngine.Client.Render.Block;
using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Client.World.LevelDomain;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable

namespace SoulboundEngine.Client.World.Render {
	public sealed class WorldRenderer {
		private readonly BlockRenderManager blockRenderManager;
		private Vector2Int lastPivot;
		private readonly RectInt renderView;
		private Func<BlockPos, BlockState> blockStateSupplier = null!;
		private readonly Tilemap tilemap;

		public WorldRenderer(RectInt renderView, BlockRenderManager blockRenderManager, Tilemap tilemap) {
			this.renderView = renderView;
			this.blockRenderManager = blockRenderManager;
			this.tilemap = tilemap;
		}

		public void RenderView(Vector2 pivot) {
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

				BlockState? blockState = this.blockStateSupplier(blockPos);
				this.RenderBlock(blockPos, blockState);
			}
		}

		private void RenderBlock(BlockPos blockPos, BlockState blockState) {
			this.blockRenderManager.Render(this.tilemap, blockPos, blockState);
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

		public void SetBlockStateSupplier(Func<BlockPos, BlockState> blockStateSupplier) {
			this.blockStateSupplier = blockStateSupplier;
		}
	}
}
