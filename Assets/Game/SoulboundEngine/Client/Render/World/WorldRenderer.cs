using SoulboundEngine.Client.Render.Block;
using SoulboundEngine.Client.Render.Entity;
using SoulboundEngine.Client.Util;
using SoulboundEngine.Common.Math;
using SoulboundEngine.World.Block;
using SoulboundEngine.World.Block.State;
using SoulboundEngine.World.Chunk;
using SoulboundEngine.World.Level;
using SoulboundEngine.World.Physics;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable

namespace SoulboundEngine.Client.Render.World {
	using Entity = SoulboundEngine.World.Entity.Entity;

	public sealed class WorldRenderer {
		private const int DEBUG_BLOCK_COLLIDER_VIEW_RADIUS = 5;
		private readonly BlockRenderManager blockRenderManager;
		private readonly EntityRenderManager entityRenderManager;
		private readonly ChunkOutlineRenderer chunkOutlineRenderer;
		private readonly Queue<(BlockPos pos, BlockState? state)> stateChangedQueue = new();
		private Vec2i lastPivot;
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

			if (this.showingChunkFeatures) {
				AABB stretched = this.level.GetPlayer().boundingBox.Stretch(DEBUG_BLOCK_COLLIDER_VIEW_RADIUS);
				foreach (AABB box in this.level.GetBlockCollisionBoxes(stretched)) {
					this.DrawDebugBox(box, Color.green);
				}
				AABB playerBox = this.level.GetPlayer().boundingBox;
				this.DrawDebugBox(playerBox, Color.cyan);
			}
		}

		private void DrawDebugBox(AABB box, Color color) {
			Vector2 min = box.Min.ToVector2();
			Vector2 max = box.Max.ToVector2();
			UnityEngine.Debug.DrawLine(new Vector3(min.x, min.y), new Vector3(min.x, max.y), color);
			UnityEngine.Debug.DrawLine(new Vector3(min.x, min.y), new Vector3(max.x, min.y), color);
			UnityEngine.Debug.DrawLine(new Vector3(max.x, min.y), new Vector3(max.x, max.y), color);
			UnityEngine.Debug.DrawLine(new Vector3(min.x, max.y), new Vector3(max.x, max.y), color);
		}

		private void RenderBlocks(Level level) {
			Vec2i currentPivot = level.GetPlayer().GetPosition().FloorToInt();
			if (this.lastPivot == currentPivot) return;

			RectInt lastView = this.ToRect(this.lastPivot);
			this.lastPivot = currentPivot;
			RectInt currentView = this.ToRect(currentPivot);

			RectInt.PositionEnumerator pos = lastView.allPositionsWithin;
			while (pos.MoveNext()) {
				if (!currentView.Contains(pos.Current)) {
					this.RenderBlock(new BlockPos(pos.Current.x, pos.Current.y), Blocks.AIR.DefaultState);
				}
			}

			pos = currentView.allPositionsWithin;
			while (pos.MoveNext()) {
				BlockPos blockPos = new(pos.Current.x, pos.Current.y);
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

		private RectInt ToRect(Vec2i pivot) {
			return new(
				Mathf.FloorToInt(pivot.x) + this.renderView.x,
				Mathf.FloorToInt(pivot.y) + this.renderView.y,
				this.renderView.width,
				this.renderView.height
			);
		}

		public bool IsInRenderView(BlockPos blockPos) {
			Vec2i pos = blockPos.ToVec2i();
			return this.ToRect(this.lastPivot).Contains(new Vector2Int(pos.x, pos.y));
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
			this.lastPivot = Vec2i.ZERO;
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
