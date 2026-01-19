using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Common;
using SoulboundBackend.Core.Resource;
using System.Resources;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable

namespace SoulboundBackend.Client.World {
	public sealed class WorldRenderer {
		private readonly Level level;
		private readonly Tilemap tilemap;
		private Vector2Int lastPivot;
		private readonly RectInt relativeRect;

		public WorldRenderer(RectInt relativeRect, Level level, Tilemap tilemap) {
			this.relativeRect = relativeRect;
			this.level = level;
			this.tilemap = tilemap;
		}

		public void RenderView(Vector2 pivot) {
			Vector2Int currentPivot = Vector2Int.FloorToInt(pivot);
			if (this.lastPivot == currentPivot) {
				return;
			}

			var lastView = ToRect(lastPivot);
			this.lastPivot = currentPivot;
			var currentView = ToRect(currentPivot);

			var pos = lastView.allPositionsWithin;
			while (pos.MoveNext()) {
				if (currentView.Contains(pos.Current)) {
					continue;
				}
				tilemap.SetTile((Vector3Int)pos.Current, null);
			}

			pos = currentView.allPositionsWithin;
			while (pos.MoveNext()) {
				BlockPos blockPos = new(pos.Current.x, pos.Current.y);
				if (!Level.IsInBounds(blockPos) || lastView.Contains(pos.Current)) {
					continue;
				}

				BlockState? blockState = level.BlockStateAt(blockPos);
				TileEntity? tileEntity = level.TileEntityAt(blockPos);

				RenderBlock(blockState, tileEntity, blockPos, tilemap);

				//blockState?.block.Render(blockState, tileEntity, blockPos, tilemap);
			}

		}

		public void RenderBlock(BlockPos blockPos, BlockState? blockState) {
			if (IsInRenderView(blockPos)) {
				RenderBlock(blockState, level.TileEntityAt(blockPos), blockPos, tilemap);

				//blockState?.block.Render(blockState, level.TileEntityAt(blockPos), blockPos, tilemap);
			}
		}

		[PROTOTYPICAL]
		private void RenderBlock(BlockState state, TileEntity? tileEntity, BlockPos pos, Tilemap tilemap) {
			
			var tile = state.block != Blocks.air
				? Core.Resource.ResourceManager.Get<TileBase, ResourceGroups.Tiles>(state.block.tileKey?.key)
				: null!;

			tilemap.SetTile((Vector3Int)pos, tile);
			tileEntity?.Render(state, tilemap);
		}

		private RectInt ToRect(Vector2Int pivot) {
			return new(
				Mathf.FloorToInt(pivot.x) + relativeRect.x,
				Mathf.FloorToInt(pivot.y) + relativeRect.y,
				relativeRect.width,
				relativeRect.height
			);
		}

		public bool IsInRenderView(BlockPos blockPos) {
			return ToRect(lastPivot).Contains((Vector2Int)blockPos);
		}
	}
}
