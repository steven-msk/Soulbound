namespace SoulboundEngine.World.Gen.Noise {
	using SoulboundEngine.Common.Collection;
	using SoulboundEngine.World.Gen.Function;
	using System;
	using System.Collections.Generic;

	public class NoiseChunk {
		private readonly Dictionary<IDensityFunction, IDensityFunction> wrappedFunctions = new();
		private readonly IDensityFunction finalTerrain;

		public NoiseChunk(RandomState randomState) {
			NoiseRouter router = randomState.Router;
			NoiseRouter wrappedRouter = router.MapAll(IDensityFunction.IVisitor.Of(this.Wrap));
			this.finalTerrain = wrappedRouter.finalTerrain;
		}

		private IDensityFunction Wrap(IDensityFunction function) {
			return this.wrappedFunctions.AddIfAbsent(function, this.WrapNew);
		}

		private IDensityFunction WrapNew(IDensityFunction function) {
			return function is DensityFunctions.CachedFunction cached
				? (cached.type switch {
					DensityFunctions.CachedFunction.Type.CACHE_LAST_X => new CacheLastX(cached.wrapped),
					_ => throw new NotImplementedException()
				})
				: function;
		}

		public double GetFinalDensity(IDensityFunction.IContext context) {
			return this.finalTerrain.Compute(context);
		}

		public record CacheLastX(IDensityFunction wrapped) : DensityFunctions.ICacheFunction {
			private int lastX = int.MinValue;
			private double lastValue;

			public DensityFunctions.CachedFunction.Type type => DensityFunctions.CachedFunction.Type.CACHE_LAST_X;

			public double Compute(IDensityFunction.IContext context) {
				int blockX = context.blockX;
				if (this.lastX == blockX) {
					return this.lastValue;
				}

				this.lastX = blockX;
				double value = this.wrapped.Compute(context);
				this.lastValue = value;
				return value;
			}

			public void FillArray(double[] output, IDensityFunction.IContextProvider contextProvider) {
				this.wrapped.FillArray(output, contextProvider);
			}
		}
	}
}
