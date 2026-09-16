namespace SoulboundEngine.World.Gen.Surface {
	using SoulboundEngine.Common.Collection;
	using SoulboundEngine.Registry;
	using SoulboundEngine.World.Block.State;
	using SoulboundEngine.World.Chunk;
	using SoulboundEngine.World.Gen.Biome;
	using SoulboundEngine.World.Level;
	using System;
	using System.Collections.Generic;
	using System.Linq;

#nullable enable

	public class SurfaceRules {
		public const int MIN_SURFACE_STEEPNESS = 4;
		public static readonly IConditionSource ON_FLOOR = new StoneDepthConditionSource(1);
		private static IConditionSource? steep = null;

		public static IConditionSource IsBiome(params RegistryKey<Biome>[] biomes) {
			return new BiomeConditionSource(biomes.ToHashSet());
		}

		public static IConditionSource UnderFloor(int maxDepth) {
			return new StoneDepthConditionSource(maxDepth);
		}

		public static IRuleSource State(BlockState blockState) {
			return new BlockRuleSource(blockState);
		}

		public static IRuleSource Sequence(params IRuleSource[] rules) {
			return rules.Length == 0
				? throw new ArgumentException("Need at least 1 rule for a sequence")
				: (IRuleSource)new SequenceRuleSource(rules.ToList());
		}

		public static IRuleSource IfTrue(IConditionSource condition, IRuleSource run) {
			return new ConditionRuleSource(condition, run);
		}

		public static IConditionSource Steep => steep ??= new SteepConditionSource();

		public sealed class Context {
			private long lastUpdateX = long.MinValue;
			private long lastUpdateY = long.MinValue;
			private int blockX, blockY;
			private RegistryEntry<Biome>? biome;
			private int stoneDepthAbove;
			private readonly Chunk chunk;
			private readonly Func<int, RegistryEntry<Biome>> biomeGetter;
			private readonly Dictionary<int, int> localXCache = new();

			public Context(Chunk chunk, Func<int, RegistryEntry<Biome>> biomeGetter) {
				this.chunk = chunk;
				this.biomeGetter = biomeGetter;
			}

			public long LastUpdateX => this.lastUpdateX;
			public long LastUpdateY => this.lastUpdateY;
			public int BlockX => this.blockX;
			public int LocalX => this.ToLocalX(this.BlockX);
			public int BlockY => this.blockY;
			public int StoneDepthAbove => this.stoneDepthAbove;
			public Chunk Chunk => this.chunk;

			public RegistryEntry<Biome> Biome => this.biome ??= this.biomeGetter(this.blockX);

			public void UpdateX(int x) {
				this.lastUpdateX++;
				this.lastUpdateY++;
				this.blockX = x;
				this.biome = null;
			}

			public void UpdateY(int y, int stoneDepthAbove) {
				this.lastUpdateY++;
				this.blockY = y;
				this.stoneDepthAbove = stoneDepthAbove;
			}

			public int ToLocalX(int x) {
				return this.localXCache.AddIfAbsent(x, this.chunk.GetPos().ToLocalX);
			}
		}

		public interface IRuleSource {
			ISurfaceRule Apply(Context context);
		}

		public interface ISurfaceRule {
			BlockState? TryApply(int blockX, int blockY);
		}

		public record SequenceRule(List<ISurfaceRule> rules) : ISurfaceRule {
			public BlockState? TryApply(int blockX, int blockY) {
				foreach (ISurfaceRule rule in this.rules) {
					BlockState? state = rule.TryApply(blockX, blockY);
					if (state != null) return state;
				}
				return null;
			}
		}

		public record SequenceRuleSource(List<IRuleSource> sequence) : IRuleSource {
			public ISurfaceRule Apply(Context context) {
				if (this.sequence.Count == 1) return this.sequence[0].Apply(context);
				List<ISurfaceRule> rules = new();
				foreach (IRuleSource rule in this.sequence) {
					rules.Add(rule.Apply(context));
				}
				return new SequenceRule(rules);
			}
		}

		public record StateRule(BlockState blockState) : ISurfaceRule {
			public BlockState? TryApply(int blockX, int blockY) => this.blockState;
		}

		public record BlockRuleSource(BlockState resultState) : IRuleSource {
			private readonly StateRule rule = new(resultState);

			public ISurfaceRule Apply(Context context) => this.rule;
		}

		public record ConditionRuleSource(IConditionSource ifTrue, IRuleSource thenRun) : IRuleSource {
			public ISurfaceRule Apply(Context context) {
				return new ConditionRule(this.ifTrue.Apply(context), this.thenRun.Apply(context));
			}
		}

		public sealed record BiomeConditionSource(HashSet<RegistryKey<Biome>> biomes) : IConditionSource {
			public ICondition Apply(Context context) => new BiomeCondition(context, this.biomes);

			private sealed class BiomeCondition : LazyXCondition {
				private readonly HashSet<RegistryKey<Biome>> biomes;

				public BiomeCondition(Context context, HashSet<RegistryKey<Biome>> biomes) 
					: base(context) {
					this.biomes = biomes;
				}

				protected override bool Compute() => this.biomes.Contains(this.context.Biome.GetKey());
			}
		}

		public sealed record StoneDepthConditionSource(int maxDepth) : IConditionSource {
			public ICondition Apply(Context context) => new StoneDepthCondition(context, this.maxDepth);

			private sealed class StoneDepthCondition : LazyYCondition {
				private readonly int maxDepth;

				public StoneDepthCondition(Context context, int maxDepth)
					: base(context) {
					this.maxDepth = maxDepth;
				}

				protected override bool Compute() => this.context.StoneDepthAbove <= this.maxDepth;
			}
		}

		public record ConditionRule(ICondition condition, ISurfaceRule rule) : ISurfaceRule {
			public BlockState? TryApply(int blockX, int blockY) {
				return !this.condition.Test() ? null : this.rule.TryApply(blockX, blockY);
			}
		}

		public interface IConditionSource {
			ICondition Apply(Context context);
		}

		public interface ICondition {
			bool Test();
		}

		public abstract class LazyCondition : ICondition {
			protected readonly Context context;
			private long lastUpdate;
			private bool? result;

			protected LazyCondition(Context context) {
				this.context = context;
				this.lastUpdate = this.GetContextLastUpdate() - 1;
			}

			public bool Test() {
				long current = this.GetContextLastUpdate();
				if (current == this.lastUpdate) return this.result!.Value;

				this.lastUpdate = current;
				this.result = this.Compute();
				return this.result.Value;
			}

			protected abstract long GetContextLastUpdate();

			protected abstract bool Compute();
		}

		public abstract class LazyXCondition : LazyCondition {
			protected LazyXCondition(Context context)
				: base(context) {
			}

			protected override long GetContextLastUpdate() => this.context.LastUpdateX;
		}

		public abstract class LazyYCondition : LazyCondition {
			protected LazyYCondition(Context context)
				: base(context) {
			}

			protected override long GetContextLastUpdate() => this.context.LastUpdateY;
		}

		private sealed record SteepConditionSource : IConditionSource {
			public ICondition Apply(Context context) => new SteepCondition(context);

			private sealed class SteepCondition : LazyXCondition {
				public SteepCondition(Context context)
					: base(context) {
				}

				protected override bool Compute() {
					int localX = this.context.LocalX;
					int xLeft = Math.Max(localX - 1, 0);
					int xRight = Math.Min(localX + 1, Level.CHUNK_LENGTH - 1);

					int heightLeft = this.context.Chunk.GetHeightmap().GetFirstFree(xLeft);
					int heightRight = this.context.Chunk.GetHeightmap().GetFirstFree(xRight);

					return Math.Abs(heightRight - heightLeft) >= MIN_SURFACE_STEEPNESS;
				}
			}
		}
	}
}
