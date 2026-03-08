using Assets.Scripts.Core.Debug.Command;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.EntitySystem;
using SoulboundBackend.Core.Debug.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Logger = SoulboundBackend.Core.Debug.Logging.Logger;

#nullable enable

namespace SoulboundBackend.Core.Debug.Commands {
	public sealed class WorldSessionCommands : ICommandProvider {
		private readonly CommandNode teleport = TeleportCommand();
		private readonly CommandNode setblock = CommandBuilder.Literal("setblock")
				.ThenCursorOf(Coords2D())
				.Then(new BlockArgumentCommandNode("block"))
				.Executes(ctx => {
					Block block = ctx.Args.Get<Block>("block");
					Vector2 playerPos = ctx.Data.Player.GetPos();
					BlockPos blockPos = new(
						Mathf.FloorToInt(ctx.Args.Get<Coordinate>("x").GetPos(playerPos.x)),
						Mathf.FloorToInt(ctx.Args.Get<Coordinate>("y").GetPos(playerPos.y))
					);
					ctx.ExecServices.Level.SetBlockState(blockPos, block.defaultState);
					Logger.LogInfo("Set block {} at {}", block.id, blockPos);
				})
				.GetRootNode();
		private readonly CommandNode spawn = SpawnCommand();
		private readonly CommandNode give = GiveCommand();

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
				.Executes(ctx => {
					Guid targetGuid = ctx.Args.TryGet("target", out Guid guid)
					   ? guid
					   : ctx.Data.Player.GetGuid();

					IEntityView target = ctx.Data.Entities.TryGetEntity(targetGuid, out var entity)
						? entity
						: ctx.Data.Player;

					Vector2 pos = target.GetPos();

					Coordinate xcoord = ctx.Args.Get<Coordinate>("x");
					Coordinate ycoord = ctx.Args.Get<Coordinate>("y");

					float x = xcoord.GetPos(pos.x);
					float y = ycoord.GetPos(pos.y);

					ctx.ExecServices.Entity.SetPos(targetGuid, new Vector2(x, y));
					Logger.LogInfo("teleported {} to x:{} y:{}", target, x, y);
				});

			CommandBuilder tp = CommandBuilder.Literal("tp");
			tp.ThenRootOf(coords);
			tp.Then(new EntityInstanceArgumentCommandNode("target"))
				.ThenRootOf(coords);
			return tp.GetRootNode();
		}

		private static CommandNode SpawnCommand() {
			static void spawnEntity(CommandParsingContext ctx) {
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

			return CommandBuilder.Literal("spawn")
				.Then(new EntityTypeArgumentCommandNode("entity"))
				.Executes(spawnEntity)
				.ThenCursorOf(Coords2D())
				.Executes(spawnEntity)
			.GetRootNode();
		}

		private static CommandNode GiveCommand() {
			static void giveItem(CommandParsingContext ctx) {
				Item item = ctx.Args.Get<Item>("item");
				int quantity = ctx.Args.TryGet("quantity", out int result) ? result : 1;
				if (quantity <= 0) {
					Logger.LogWarning("Quantity must be positive");
					return;
				}

				Dictionary<int, ItemStack?> stackSnapshots = ctx.Data.Player.GetInventory().stacks;
				int fullStacks = quantity / item.fullStackSize;
				int remainder = quantity % item.fullStackSize;

				// get stacks
				int stackCount = fullStacks + (remainder > 0 ? 1 : 0);
				ItemStack[] stacks = new ItemStack[stackCount];
				int i;
				for (i = 0; i < fullStacks; i++) {
					stacks[i] = item.CreateStack(item.fullStackSize);
				}
				if (remainder > 0) stacks[i] = item.CreateStack(remainder);

				// flow to stackable
				List<int> flowSlots = stackSnapshots
					.Where(kvp => kvp.Value?.item == item)
					.Where(kvp => kvp.Value!.GetSpaceLeft() > 0)
					.Select(kvp => kvp.Key)
					.ToList();
				int currentStack = 0;
				foreach (var slot in flowSlots) {
					ItemStack stack = stackSnapshots[slot]!;
					if (!stack.IsStackableWith(stacks[currentStack])) continue;

					stack.FillFrom(stacks[currentStack]);
					ctx.ExecServices.Player.Inventory.SetStack(slot, stack);

					if (stacks[currentStack].IsEmpty()) currentStack++;
					if (currentStack >= stacks.Length) break;
				}

				// refill remaining stacks
				for (i = 0; i < stacks.Length - 1; i++) {
					for (int j = i + 1; j < stacks.Length; j++) {
						stacks[i].FillFrom(stacks[j]);
					}
				}

				// flow to empty slots
				List<int> emptySlots = ctx.Data.Player.GetInventory().stacks
					.Where(kvp => kvp.Value?.IsEmpty() ?? true)
					.Select(kvp => kvp.Key)
					.ToList();
				currentStack = 0;
				foreach (var slot in emptySlots) {
					ItemStack stack = stacks[currentStack];
					if (stack.IsEmpty()) continue;

					ctx.ExecServices.Player.Inventory.SetStack(slot, stack);
					currentStack++;

					if (currentStack >= stacks.Length) break;
				}

				if (stacks.Any(s => !s.IsEmpty())) {
					Logger.LogWarning("/give stacks exceeded capacity");
				}
			}

			return CommandBuilder.Literal("give")
				.Then(new ItemArgumentCommandNode("item"))
				.Executes(giveItem)
				.Then(new ArgumentCommandNode<int>("quantity", new IntParser()))
				.Executes(giveItem)
			.GetRootNode();
		}
	}
}
