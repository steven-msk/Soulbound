namespace SoulboundEngine.Item {
	using SoulboundEngine.Common;
	using SoulboundEngine.Serialization;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Block.State;
	using System.Collections.Generic;
	using System.Linq;

	public record Tool(List<Tool.Rule> rules, int durabilityCost) {
		public static readonly Codec<Tool> CODEC = RecordCodec<Tool, List<Rule>, int>.Of(
			Field.Required<Tool, List<Rule>>("rules", Rule.CODEC.ListOf(), t => t.rules),
			Field.Required<Tool, int>("durabilityCost", Codecs.INT, t => t.durabilityCost),
			(rules, damagePerBlock) => new Tool(rules, damagePerBlock)
		);

		public float? GetMiningSpeed(BlockState state) {
			foreach (Rule rule in this.rules) {
				if (rule.speed.HasValue && rule.blocks.Contains(state.block)) {
					return rule.speed.Value;
				}
			}
			return null;
		}

		public bool CanMine(BlockState state) {
			foreach (Rule rule in this.rules) {
				if (rule.canBreak.HasValue && rule.blocks.Contains(state.block)) {
					return rule.canBreak.Value;
				}
			}
			return false;
		}

		public record Rule(HashSet<Block> blocks, float? speed, bool? canBreak) {
			public static readonly Codec<Rule> CODEC = RecordCodec<Rule, List<Block>, UnmanagedOptional<float>, UnmanagedOptional<bool>>.Of(
				Field.Required<Rule, List<Block>>("blocks", Block.CODEC.ListOf(), r => r.blocks.ToList()),
				Field.Required<Rule, UnmanagedOptional<float>>("speed", Codecs.FLOAT.MakeOptional<float>(), r => UnmanagedOptional<float>.Of(r.speed)),
				Field.Required<Rule, UnmanagedOptional<bool>>("canBreak", Codecs.BOOLEAN.MakeOptional<bool>(), r => UnmanagedOptional<bool>.Of(r.canBreak)),
				(blocks, speed, canBreak) => new Rule(blocks.ToHashSet(), speed.GetAsIs(), canBreak.GetAsIs())
			);

			public static Rule Mines(IEnumerable<Block> blocks, float speed) {
				return new Rule(blocks.ToHashSet(), speed, true);
			}

			public static Rule CantMine(IEnumerable<Block> blocks) {
				return new Rule(blocks.ToHashSet(), null, false);
			}

			public static Rule OverridesSpeed(IEnumerable<Block> blocks, float speed) {
				return new Rule(blocks.ToHashSet(), speed, null);
			}
		}
	}
}
