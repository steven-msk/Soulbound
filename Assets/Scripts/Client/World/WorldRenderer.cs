using SoulboundBackend.Client.World.BlockSystem;
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

		public void Render(Vector2 pivot) {
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
				if (!level.IsInBounds(blockPos) || lastView.Contains(pos.Current)) {
					continue;
				}

				BlockState? blockState = level.BlockStateAt(blockPos);
				TileEntity? tileEntity = level.TileEntityAt(blockPos);

				blockState?.block.Render(blockState, tileEntity, blockPos, tilemap);
			}

		}

		private RectInt ToRect(Vector2Int pivot) {
			return new(
				Mathf.FloorToInt(pivot.x) + relativeRect.x,
				Mathf.FloorToInt(pivot.y) + relativeRect.y,
				relativeRect.width,
				relativeRect.height
			);
		}
	}
}
