using SoulboundEngine.Client.Render.Block;
using SoulboundEngine.Client.Render.Entity;
using SoulboundEngine.Client.World.Block;
using SoulboundEngine.Client.World.Block.State;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable

namespace SoulboundEngine.Client.World.Render {
	using Entity = Entity.Entity;
	using Level = Level.Level;
	using Logger = Debug.Logging.Logger;

	public sealed class WorldRenderer {
		private readonly BlockRenderManager blockRenderManager;
		private readonly EntityRenderManager entityRenderManager;
		private Vector2Int lastPivot;
		private readonly RectInt renderView;
		private Tilemap? tilemap;
		private Level? level;

		public WorldRenderer(RectInt renderView, BlockRenderManager blockRenderManager, EntityRenderManager entityRenderManager) {
			this.renderView = renderView;
			this.blockRenderManager = blockRenderManager;
			this.entityRenderManager = entityRenderManager;
		}

		// NOTE: current implementation relies on single tilemap rendering (one tilemap for the entire render view)
		// later optimizations can replace this with tilemap render buffers or per-chunk tilemap meshes

		public void Render() {
			if (this.level == null) return;

			this.RenderBlocks(this.level);
			this.RenderEntities(this.level);
		}

		private void RenderBlocks(Level level) {
			Vector2Int currentPivot = Vector2Int.FloorToInt(level.GetPlayer().GetPosition());
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

				BlockState? blockState = level.GetBlockState(blockPos);
				this.RenderBlock(blockPos, blockState);
			}
		}

		private void RenderEntities(Level level) {
			foreach (var entity in level.GetAllEntities()) {
				this.entityRenderManager.Update(entity);
			}
			// temporary hook
			level.GetPlayer().FrameUpdate();
		}

		private void RenderBlock(BlockPos blockPos, BlockState? blockState) {
			if (this.tilemap == null) {
				throw new InvalidOperationException("Cannot render block: tilemap is null");
			}
			this.blockRenderManager.Render(this.tilemap, blockPos, blockState);
		}

		private void BlockStateChanged(BlockPos blockPos, BlockState? oldState, BlockState? newState) {
			if (!this.IsInRenderView(blockPos)) return;
			this.RenderBlock(blockPos, newState);
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

		private void EntityAdded(Entity entity) {
			Logger.LogInfo("entity added: {}", entity);
			this.entityRenderManager.Render(entity);
		}

		private void EntityRemoved(Entity entity) {
			this.entityRenderManager.Destroy(entity);
		}

		public void SetLevel(Level? level) {
			this.RemoveLevelEvents();
			this.level = level;
			this.AddLevelEvents();
		}

		private void AddLevelEvents() {
			if (this.level == null) return;
			this.level.blockStateChanged += this.BlockStateChanged;
			this.level.entityAdded += this.EntityAdded;
			this.level.entityRemoved += this.EntityRemoved;
		}

		private void RemoveLevelEvents() {
			if (this.level == null) return;
			this.level.blockStateChanged -= this.BlockStateChanged;
			this.level.entityAdded -= this.EntityAdded;
			this.level.entityRemoved -= this.EntityRemoved;
		}

		public void SetTilemap(Tilemap tilemap) {
			if (this.tilemap != null) {
				this.tilemap.ClearAllTiles();
			}
			this.tilemap = tilemap;
		}
	}
}
