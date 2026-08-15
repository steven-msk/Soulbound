using SoulboundEngine.Client.Render.Block;
using SoulboundEngine.Client.Render.Entity;
using SoulboundEngine.World.Block;
using SoulboundEngine.World.Block.State;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable

namespace SoulboundEngine.Client.World.Render {
	using Chunk = Chunk.Chunk;
	using Entity = Entity.Entity;
	using Level = Level.Level;

	public sealed class WorldRenderer {
		private readonly BlockRenderManager blockRenderManager;
		private readonly EntityRenderManager entityRenderManager;
		private readonly ChunkOutlineRenderer chunkOutlineRenderer;
		private readonly Queue<(BlockPos pos, BlockState? state)> stateChangedQueue = new();
		private Vector2Int lastPivot;
		private readonly RectInt renderView;
		private Tilemap? tilemap;
		private Level? level;
		private bool showingChunkFeatures;

		public WorldRenderer(RectInt renderView, BlockRenderManager blockRenderManager, EntityRenderManager entityRenderManager) {
			this.renderView = renderView;
			this.blockRenderManager = blockRenderManager;
			this.entityRenderManager = entityRenderManager;
			this.chunkOutlineRenderer = new ChunkOutlineRenderer();
		}

		// NOTE: current implementation relies on single tilemap rendering (one tilemap for the entire render view)
		// later optimizations can replace this with tilemap render buffers or per-chunk tilemap meshes

		public void Render() {
			if (this.level == null) return;

			this.RenderBlocks(this.level);
			this.UpdateEntities(this.level);

			this.ResolveQueue(this.stateChangedQueue, value => {
				this.RenderBlock(value.pos, value.state);
			});
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

		private void RenderBlock(BlockPos blockPos, BlockState? blockState) {
			if (this.tilemap == null) {
				throw new InvalidOperationException("Cannot render block: tilemap is null");
			}
			this.blockRenderManager.Render(this.tilemap, blockPos, blockState);
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

		private void ResolveQueue<T>(Queue<T> queue, Action<T> action) {
			if (queue.TryDequeue(out T value)) {
				action(value);
			}
		}

		private void BlockStateChanged(BlockPos blockPos, BlockState? oldState, BlockState? newState) {
			if (!this.IsInRenderView(blockPos)) return;
			this.stateChangedQueue.Enqueue((blockPos, newState));
		}

		private void EntityAdded(Entity entity) {
			this.RenderEntity(entity);
		}

		private void EntityRemoved(Entity entity) {
			this.DestroyEntity(entity);
		}

		private void RenderEntities(Level level) {
			foreach (var entity in level.GetAllEntities()) {
				this.RenderEntity(entity);
			}
		}

		private void DestroyEntities(Level level) {
			foreach (var entity in level.GetAllEntities()) {
				this.DestroyEntity(entity);
			}
		}

		private void UpdateEntities(Level level) {
			foreach (var entity in level.GetAllEntities()) {
				this.UpdateEntity(entity);
			}
		}

		private void RenderEntity(Entity entity) {
			this.entityRenderManager.Render(entity);
		}

		private void DestroyEntity(Entity entity) {
			this.entityRenderManager.Destroy(entity);
		}

		private void UpdateEntity(Entity entity) {
			this.entityRenderManager.Update(entity);
		}

		private void OnChunkLoaded(Chunk chunk) {
			if (this.showingChunkFeatures) {
				this.chunkOutlineRenderer.ShowOutline(chunk);
			}
		}

		private void OnChunkUnloaded(Chunk chunk) {
			this.chunkOutlineRenderer.HideOutline(chunk);
		}

		public void ShowChunkFeatures() {
			this.showingChunkFeatures = true;
			if (this.level == null) return;
			foreach (var chunk in this.level.GetLoadedChunks()) {
				this.chunkOutlineRenderer.ShowOutline(chunk);
			}
		}

		public void HideChunkFeatures() {
			this.showingChunkFeatures = false;
			this.chunkOutlineRenderer.Clear();
		}

		public void SetLevel(Level? level) {
			this.RemoveLevelEvents();
			if (this.level != null) this.DestroyEntities(this.level);
			this.level = level;
			if (level != null) this.RenderEntities(level);
			this.AddLevelEvents();
		}

		public void Reset() {
			this.lastPivot = Vector2Int.zero;
			if (this.level != null) this.DestroyEntities(this.level);
			if (this.tilemap != null) this.tilemap.ClearAllTiles();
			this.showingChunkFeatures = false;
		}

		private void AddLevelEvents() {
			if (this.level == null) return;
			this.level.blockStateChanged += this.BlockStateChanged;
			this.level.entityAdded += this.EntityAdded;
			this.level.entityRemoved += this.EntityRemoved;
			this.level.chunkLoaded += this.OnChunkLoaded;
			this.level.chunkUnloaded += this.OnChunkUnloaded;
		}

		private void RemoveLevelEvents() {
			if (this.level == null) return;
			this.level.blockStateChanged -= this.BlockStateChanged;
			this.level.entityAdded -= this.EntityAdded;
			this.level.entityRemoved -= this.EntityRemoved;
			this.level.chunkLoaded -= this.OnChunkLoaded;
			this.level.chunkUnloaded -= this.OnChunkUnloaded;
		}

		public void SetTilemap(Tilemap tilemap) {
			if (this.tilemap != null) {
				this.tilemap.ClearAllTiles();
			}
			this.tilemap = tilemap;
		}
	}
}
