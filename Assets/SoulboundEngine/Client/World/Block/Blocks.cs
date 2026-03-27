using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Common;
using SoulboundEngine.Core.Assets;

using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using SoulboundEngine.Common.Patterns;
using SoulboundEngine.Core;

namespace SoulboundEngine.Client.World.BlockSystem {
	public partial class Blocks {
		// contract initializer, do not modify
		private static readonly IRegistrationContract<Block, IRegistrationKey<Block>> _contract = Registry<Block>.SetContract(new BlockRegistrationContract());

		public static readonly Block air = Registry<Block>.Add(new AirBlock());
		public static readonly Block grass = Registry<Block>.Add(new GenericBlock("grass", "Grass Block", new AssetKey("grass"), 0));
		public static readonly Block dirt = Registry<Block>.Add(new GenericBlock("dirt", "Dirt Block", new AssetKey("dirt"), 0));
		public static readonly Block stone = Registry<Block>.Add(new GenericBlock("stone", "Stone Block", new AssetKey("stone"), 1));
		public static readonly Block wood = Registry<Block>.Add(new GenericBlock("wood", "Wood", new AssetKey("wood"), 0));
		public static readonly Block leaves = Registry<Block>.Add(new LeafBlock());

		public static readonly ToggleBlock toggleBlock = Registry<Block>.Add(new ToggleBlock());
		public static readonly NeighborReactiveBlock neighborReactiveBlock = Registry<Block>.Add(new NeighborReactiveBlock());
		public static readonly TickingBlock tickingBlock = Registry<Block>.Add(new TickingBlock());
		public static readonly PulseBlock pulseBlock = Registry<Block>.Add(new PulseBlock());
		public static readonly SelfDestructBlock selfDestructBlock = Registry<Block>.Add(new SelfDestructBlock());
		public static readonly MovingTickingBlock movingTickingBlock = Registry<Block>.Add(new MovingTickingBlock());
		public static readonly AreaTriggerBlock areaTriggerBlock = Registry<Block>.Add(new AreaTriggerBlock());

		public sealed class BlockRegistrationContract : IRegistrationContract<Block, IRegistrationKey<Block>> {
			public IRegistrationKey<Block> ValueToKey(Block value) {
				return new Block.RegistrationKey(value.GetID());
			}
		}
	}
}
