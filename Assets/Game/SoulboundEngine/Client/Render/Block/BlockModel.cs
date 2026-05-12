using SoulboundEngine.Client.World.BlockSystem.States;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SoulboundEngine.Client.Render.Block {
	public class BlockModel {
		public readonly TileBase tile;
		public readonly Color color;

		public static BlockModel AIR = new(null, Color.white);

		public BlockModel(TileBase tile, Color color) {
			this.tile = tile;
			this.color = color;
		}

		public BlockModel(TileBase tile)
			: this(tile, Color.white) {
		}

		public interface IFactory {
			BlockModel Create(BlockState blockState);

			public static IFactory Of(Func<BlockState, BlockModel> modelFactory) {
				return new FuncImpl(modelFactory);
			}

			private sealed class FuncImpl : IFactory {
				private readonly Func<BlockState, BlockModel> modelFactory;

				public FuncImpl(Func<BlockState, BlockModel> modelFactory) {
					this.modelFactory = modelFactory;
				}

				public BlockModel Create(BlockState blockState) {
					return this.modelFactory(blockState);
				}
			}
		}
	}
}
