namespace SoulboundEngine.UnityClient.Render.World {
	using SoulboundEngine.Common.Math;
	using SoulboundEngine.UnityClient.Debug;
	using SoulboundEngine.UnityClient.Render.Block;
	using SoulboundEngine.UnityClient.Render.Entity;
	using SoulboundEngine.UnityClient.Util;
	using SoulboundEngine.UnityClient.World.Widget;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Block.State;
	using SoulboundEngine.World.Chunk;
	using SoulboundEngine.World.Entity;
	using SoulboundEngine.World.Level;
	using SoulboundEngine.World.Physics;
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Tilemaps;

#nullable enable

	public sealed class WorldRenderer {
		private const int DEBUG_BLOCK_COLLIDER_VIEW_RADIUS = 5;
		private readonly BlockRenderManager blockRenderManager;
		private readonly EntityRenderManager entityRenderManager;
		private readonly WorldWidgetManager widgetManager;
		private readonly DebugRenderer debugRenderer;
		private readonly ChunkOutlineRenderer chunkOutlineRenderer;
		private readonly BlockBreakProgressRenderer blockBreakProgressRenderer;
		private readonly Queue<(BlockPos pos, BlockState? state)> stateChangedQueue = new();
		private Vec2i lastPivot = Vec2i.ZERO;
		private readonly RectInt renderView;
		private Tilemap? tilemap;
		private Level? level;
		private bool showingChunkFeatures;

		public WorldRenderer(RectInt renderView, BlockRenderManager blockRenderManager, EntityRenderManager entityRenderManager, WorldWidgetManager widgetManager, DebugRenderer debugRenderer) {
			this.renderView = renderView;
			this.blockRenderManager = blockRenderManager;
			this.entityRenderManager = entityRenderManager;
			this.widgetManager = widgetManager;
			this.debugRenderer = debugRenderer;
			this.chunkOutlineRenderer = new ChunkOutlineRenderer(debugRenderer);
			this.blockBreakProgressRenderer = new BlockBreakProgressRenderer();
		}

		// NOTE: current implementation relies on single tilemap rendering (one tilemap for the entire render view)
		// later optimizations can replace this with tilemap render buffers or per-chunk tilemap meshes

		public void Render() {
			if (this.level == null) return;

			this.RenderBlocks(this.level);
			this.UpdateEntities(this.level);

			this.ResolveQueue(this.stateChangedQueue, value => {
				this.RenderBlock(value.pos.x, value.pos.y, value.state);
			});
			this.blockBreakProgressRenderer.Render();

			if (this.showingChunkFeatures) {
				this.chunkOutlineRenderer.Render();

				AABB stretched = this.level.GetPlayer().boundingBox.Stretch(DEBUG_BLOCK_COLLIDER_VIEW_RADIUS);
				foreach (AABB box in this.level.GetBlockCollisionBoxes(stretched)) {
					this.debugRenderer.AddLineBox(box, Color.green);
				}
				foreach (Entity entity in this.level.GetAllEntities()) {
					AABB boundingBox = entity.boundingBox;
					Common.Color color = entity.GetDescriptor().GetCategory().debugBoxColor;
					this.debugRenderer.AddLineBox(boundingBox, color.ToUnityColor());
				}
			}
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
					this.RenderBlock(pos.Current.x, pos.Current.y, Blocks.AIR.DefaultState);
				}
			}

			pos = currentView.allPositionsWithin;
			while (pos.MoveNext()) {
				BlockPos blockPos = new(pos.Current.x, pos.Current.y);
				if (!Level.IsInBounds(blockPos) || lastView.Contains(pos.Current)) {
					continue;
				}

				BlockState? blockState = level.GetBlockState(blockPos);
				this.RenderBlock(blockPos.x, blockPos.y, blockState);
			}
		}

		private void RefreshAllBlocks() {
			if (this.level == null) return;
			RectInt renderView = this.ToRect(this.lastPivot);
			RectInt.PositionEnumerator pos = renderView.allPositionsWithin;
			while (pos.MoveNext()) {
				BlockPos blockPos = new(pos.Current.x, pos.Current.y);
				this.RenderBlock(pos.Current.x, pos.Current.y, this.level.GetBlockState(blockPos));
			}
		}

		private void RenderBlock(int x, int y, BlockState? blockState) {
			if (this.tilemap == null) {
				throw new InvalidOperationException("Cannot render block: tilemap is null");
			}
			this.blockRenderManager.Render(this.tilemap, x, y, blockState);
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
			foreach (Entity entity in level.GetAllEntities()) {
				this.RenderEntity(entity);
			}
		}

		private void DestroyEntities(Level level) {
			foreach (Entity entity in level.GetAllEntities()) {
				this.DestroyEntity(entity);
			}
		}

		private void UpdateEntities(Level level) {
			foreach (Entity entity in level.GetAllEntities()) {
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
			this.RefreshAllBlocks();
		}

		private void OnChunkUnloaded(Chunk chunk) {
			this.chunkOutlineRenderer.HideOutline(chunk);
			this.RefreshAllBlocks();
		}

		public void ShowChunkFeatures() {
			this.showingChunkFeatures = true;
			if (this.level == null) return;
			foreach (Chunk chunk in this.level.GetLoadedChunks()) {
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
			this.widgetManager.SetLevel(level);
			if (level != null) {
				this.RenderEntities(level);
				this.blockBreakProgressRenderer.SetPlayer(level.GetPlayer());
			}
			this.blockBreakProgressRenderer.Reset();
			this.AddLevelEvents();
			this.chunkOutlineRenderer.Clear();
		}

		public void Reset() {
			this.lastPivot = Vec2i.ZERO;
			if (this.level != null) this.DestroyEntities(this.level);
			if (this.tilemap != null) this.tilemap.ClearAllTiles();
			this.showingChunkFeatures = false;
			this.chunkOutlineRenderer.Clear();
			this.blockBreakProgressRenderer.Reset();
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
