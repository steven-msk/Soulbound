using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Client.World.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Logger = SoulboundEngine.Client.Debug.Logging.Logger;

#nullable enable

namespace SoulboundEngine.Client.Debug.Commands {
	public sealed class WorldSessionCommands : ICommandProvider {
		private readonly CommandNode teleport = TeleportCommand();
		private readonly CommandNode setblock = CommandBuilder.Literal("setblock")
				.ThenCursorOf(Coords2D())
					.Then(new ArgumentCommandNode<Block>("block", new BlockIDParser(), new BlockCompletionSupplier()))
						.Executes(ctx => {
							Block block = ctx.Args.Get<Block>("block");
							Vector2 playerPos = ctx.Data.Player.GetPos();
							BlockPos blockPos = new(
								Mathf.FloorToInt(ctx.Args.Get<Coordinate>("x").GetPos(playerPos.x)),
								Mathf.FloorToInt(ctx.Args.Get<Coordinate>("y").GetPos(playerPos.y))
							);
							ctx.ExecServices.Level.SetBlockState(blockPos, block.defaultState);
							Logger.LogInfo("Set block {} at {}", block.GetID(), blockPos);
						})
			.GetRootNode();
		private readonly CommandNode spawn = CommandBuilder.Literal("spawn")
				.Then(new ArgumentCommandNode<EntityDescriptor>("entity", new EntityDescriptorParser(), new EntityTypeCompletionSupplier()))
					.Executes(SpawnEntity)
				.ThenCursorOf(Coords2D())
					.Executes(SpawnEntity)
			.GetRootNode();
		private readonly CommandNode give = CommandBuilder.Literal("give")
				.Then(new ArgumentCommandNode<Item>("item", new ItemParser(), new ItemCompletionSupplier()))
					.Executes(GiveItem)
				.Then(new ArgumentCommandNode<int>("quantity", new IntParser()))
					.Executes(GiveItem)
			.GetRootNode();

		public IEnumerable<CommandNode> GetCommands() {
			yield return teleport;
			yield return setblock;
			yield return spawn;
			yield return give;
		}

		private static CommandBuilder Coords2D() {
			return CommandBuilder.Create(new ArgumentCommandNode<Coordinate>("x", new CoordinateParser()))
				.Then(new ArgumentCommandNode<Coordinate>("y", new CoordinateParser()));
		}

		private static CommandNode TeleportCommand() {
			CommandBuilder coords = Coords2D()
				.Executes(Teleport);

			CommandBuilder tp = CommandBuilder.Literal("tp");
			tp.ThenRootOf(coords);
			tp.Then(new ArgumentCommandNode<Guid>("target", new EntityGUIDParser(), new EntityInstanceCompletionSupplier()))
				.ThenRootOf(coords);
			return tp.GetRootNode();
		}

		private static void Teleport(CommandParsingContext context) {
			Guid targetGuid = context.Args.TryGet("target", out Guid guid)
					   ? guid
					   : context.Data.Player.GetGuid();

			IEntityView target = context.Data.Entities.TryGetEntity(targetGuid, out var entity)
				? entity
				: context.Data.Player;

			Vector2 pos = target.GetPos();

			Coordinate xcoord = context.Args.Get<Coordinate>("x");
			Coordinate ycoord = context.Args.Get<Coordinate>("y");

			float x = xcoord.GetPos(pos.x);
			float y = ycoord.GetPos(pos.y);

			context.ExecServices.Entity.SetPos(targetGuid, new Vector2(x, y));
			Logger.LogInfo("teleported {} to x:{} y:{}", target, x, y);
		}

		private static void SpawnEntity(CommandParsingContext ctx) {
			EntityDescriptor entityDescriptor = ctx.Args.Get<EntityDescriptor>("entity");
			Vector2 pos = ctx.Data.Player.GetPos();

			Coordinate? xcoord = ctx.Args.TryGet<Coordinate>("x", out var x) ? x : null;
			Coordinate? ycoord = ctx.Args.TryGet<Coordinate>("y", out var y) ? y : null;
			if (xcoord != null && ycoord != null) {
				pos = new Vector2(xcoord.Value.GetPos(pos.x), ycoord.Value.GetPos(pos.y));
			}

			Entity entity = entityDescriptor.Create(pos);
			ctx.ExecServices.Entity.AddEntity(entity);
		}

		private static void GiveItem(CommandParsingContext ctx) {
			Item item = ctx.Args.Get<Item>("item");
			int quantity = ctx.Args.TryGet("quantity", out int result) ? result : 1;
			if (quantity <= 0) {
				Logger.LogError("Quantity must be positive");
				return;
			}

			int fullStacks = quantity / item.fullStackSize;
			int remainder = quantity % item.fullStackSize;

			int stackCount = fullStacks + (remainder > 0 ? 1 : 0);
			ItemStack[] stacks = new ItemStack[stackCount];
			int i;
			for (i = 0; i < fullStacks; i++) {
				stacks[i] = item.CreateStack(item.fullStackSize);
			}
			if (remainder > 0) stacks[i] = item.CreateStack(remainder);

			for (int j = 0; j < stacks.Length; j++) {
				ctx.ExecServices.Player.TryAddItemStack(stacks[j]);
			}
		}

	}
}
