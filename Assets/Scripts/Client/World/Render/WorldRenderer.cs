using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.BlockSystem.States;
using SoulboundBackend.Client.World.BlockSystem.TileEntities;
using SoulboundBackend.Client.World.LevelDomain;
using SoulboundBackend.Common;
using SoulboundBackend.Core.Assets;

using System.Resources;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable

namespace SoulboundBackend.Client.World.Render {
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
			if (this.lastPivot == currentPivot) return;

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

				BlockState? blockState = level.GetBlockState(blockPos);
				TileEntity? tileEntity = level.TileEntityAt(blockPos);

				RenderBlock(blockState, tileEntity, blockPos, tilemap);
			}

		}

		public void RenderBlock(BlockPos blockPos, BlockState? blockState) {
			if (IsInRenderView(blockPos)) {
				RenderBlock(blockState, level.TileEntityAt(blockPos), blockPos, tilemap);
			}
		}

		[PROTOTYPICAL]
		private void RenderBlock(BlockState? state, TileEntity? tileEntity, BlockPos pos, Tilemap tilemap) {
			state ??= Blocks.air.defaultState;
			AssetKey tileKey = state.block.GetRenderTileKey(state);
			TileBase? tile = tileKey != null
				? AssetManager.Resolve<TileBase>(tileKey)
				: null;

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
