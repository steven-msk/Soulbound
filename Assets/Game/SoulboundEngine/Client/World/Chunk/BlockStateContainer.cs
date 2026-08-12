using SoulboundEngine.Client.World.Block.State;
using System.Collections;
using System.Collections.Generic;

#nullable enable

namespace SoulboundEngine.Client.World.Chunk {
	using Block = Block.Block;

	public sealed class BlockStateContainer : IEnumerable<BlockState> {
		private readonly List<int> palette = new() { 0 };
		private readonly Dictionary<int, int> paletteLookup = new() { [0] = 0 };
		private readonly int[][] indices;
		private int nonAirCount;
		public readonly int width;
		public readonly int height;

		// impl detail:
		// the palette starts local, and stays local for the rest of the session.
		// as a future optimization, one the palette grows past a size threshold,
		// (once the bits needed to index the local palette would be the same as using global ids directly)
		// it swaps this section to global indexing, since local paletting stopped paying for itself at that point.
		// note this as a beta roadmap point

		public BlockStateContainer(int width, int height) {
			this.width = width;
			this.height = height;

			this.indices = new int[width][];
			for (int i = 0; i < this.indices.Length; i++) {
				this.indices[i] = new int[height];
			}
		}

		public bool HasOnlyAir => this.nonAirCount == 0;

		public BlockState Get(int localX, int localY) {
			int paletteIndex = this.indices[localX][localY];
			return Block.GetState(this.palette[paletteIndex]);
		}

		public void Set(int localX, int localY, BlockState state) {
			int stateId = Block.GetRawID(state);
			int oldStateId = this.palette[this.indices[localX][localY]];

			if (oldStateId != 0 && stateId == 0) this.nonAirCount--;
			else if (oldStateId == 0 && stateId != 0) this.nonAirCount++;

			this.indices[localX][localY] = this.GetOrAddPaletteIndex(stateId);
		}

		private int GetOrAddPaletteIndex(int stateId) {
			if (this.paletteLookup.TryGetValue(stateId, out int index)) return index;

			index = this.palette.Count;
			this.palette.Add(stateId);
			this.paletteLookup[stateId] = index;
			return index;
		}

		public IEnumerator<BlockState> GetEnumerator() {
			foreach (var array in this.indices) {
				foreach (var index in array) {
					yield return Block.GetState(this.palette[index]);
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
	}
}
