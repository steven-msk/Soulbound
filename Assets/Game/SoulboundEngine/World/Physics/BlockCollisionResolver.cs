namespace SoulboundEngine.World.Physics {
	using SoulboundEngine.Common;
	using SoulboundEngine.Common.Math;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Block.State;
	using SoulboundEngine.World.Chunk;
	using SoulboundEngine.World.Level;
	using System.Collections;
	using System.Collections.Generic;

#nullable enable

	public class BlockCollisionResolver : IEnumerable<AABB> {
		private readonly Level level;
		private readonly Cursor2D cursor;
		private readonly AABB box;
		private BlockPos blockPos;

		public BlockCollisionResolver(Level level, AABB box) {
			this.level = level;
			this.box = box;
			this.blockPos = new BlockPos();
			int minX = Maths.FloorToInt(box.minX - 1.0E-7) - 1;
			int maxX = Maths.FloorToInt(box.maxX + 1.0E-7) + 1;
			int minY = Maths.FloorToInt(box.minY - 1.0E-7) - 1;
			int maxY = Maths.FloorToInt(box.maxY + 1.0E-7) + 1;
			this.cursor = new Cursor2D(minX, minY, maxX, maxY);
		}

		public IEnumerator<AABB> GetEnumerator() {
			return this.Resolve().GetEnumerator();
		}

		public IEnumerable<AABB> Resolve() {
			while (this.cursor.Advance()) {
				int x = this.cursor.NextX();
				int y = this.cursor.NextY();
				int faceType = this.cursor.GetNextType();
				if (faceType == Cursor2D.CORNER || faceType == Cursor2D.EDGE) continue;

				Chunk? chunk = this.level.ChunkAt(x);
				if (chunk == null) continue;

				this.blockPos.x = x;
				this.blockPos.y = y;
				BlockState blockState = chunk.GetBlockState(this.blockPos);
				BlockShape blockShape = blockState.GetCollisionShape(blockState, this.level, this.blockPos);

				AABB[] boxes = new AABB[blockShape.boxes.Count];
				if (blockShape.IsEmpty || !Intersect(this.box, blockShape, this.blockPos, ref boxes, out int count)) continue;
				for (int i = 0; i < count; i++) {
					yield return boxes[i];
				}
			}
		}

		public static bool Intersect(AABB box, BlockShape blockShape, BlockPos blockPos, ref AABB[] intersecting, out int count) {
			count = 0;
			foreach (var shapeBox in blockShape.boxes) {
				AABB movedBox = shapeBox.Move(blockPos);
				if (box.Intersects(movedBox)) {
					intersecting[count++] = movedBox;
				}
			}
			return count > 0;
		}

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
	}
}
