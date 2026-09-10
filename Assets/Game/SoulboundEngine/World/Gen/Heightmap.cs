namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Block.State;
	using SoulboundEngine.World.Chunk;
	using System;

#nullable enable

	public sealed class Heightmap {
		private readonly int size;
		private readonly int[] heights;
		private readonly Predicate<BlockState> isOpaque;
		private readonly Chunk chunk;

		public Heightmap(int size, Predicate<BlockState> isOpaque, Chunk chunk) {
			this.size = size;
			this.isOpaque = isOpaque;
			this.chunk = chunk;
			this.heights = new int[size];
			Array.Fill(this.heights, chunk.GetBottomY());
		}

		private static int GetIndex(int localX) => localX;

		public bool Update(int localX, int localY, BlockState blockState) {
			int index = GetIndex(localX);
			int firstFree = this.GetFirstFreeFromIndex(index);

			if (this.isOpaque(blockState)) {
				if (localY + 1 > firstFree) {
					this.SetHeight(localX, localY + 1);
					return true;
				}
				return false;
			}
			BlockPos.Mutable pos = new();

			if (localY + 1 == firstFree) {
				for (int y = localY - 1; y >= this.chunk.GetBottomY(); y--) {
					pos.Set(this.chunk.GetPos().ToWorldX(localX), y);
					if (this.isOpaque(this.chunk.GetBlockState(pos))) {
						this.SetHeight(localX, localY + 1);
						return true;
					}
				}

				this.SetHeight(localX, this.chunk.GetBottomY());
				return true;
			}

			return false;
		}

		private int GetFirstFreeFromIndex(int index) {
			return this.heights[index] + this.chunk.GetBottomY();
		}

		public int GetFirstFree(int localX) => this.GetFirstFree(GetIndex(localX));

		public void SetRaw(int[] data) {
			if (data.Length != this.size) {
				throw new ArgumentException($"Mismatched heightmap array length: expected {this.size}, got {data.Length}");
			}
			Array.Copy(data, this.heights, this.size);
		}

		public int[] GetRawImmutable() => (int[])this.heights.Clone();

		private void SetHeight(int x, int height) {
			this.heights[GetIndex(x)] = height - this.chunk.GetBottomY();
		}
	}
}
