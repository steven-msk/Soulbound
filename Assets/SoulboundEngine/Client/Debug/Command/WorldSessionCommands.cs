using Brigadier.NET;
using Brigadier.NET.Builder;
using Brigadier.NET.Context;
using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Client.World.EntitySystem;
using System;
using UnityEngine;
using Logger = SoulboundEngine.Client.Debug.Logging.Logger;

#nullable enable

namespace SoulboundEngine.Client.Debug.Commands {
	public sealed class WorldSessionCommands : ICommandProvider {
		void ICommandProvider.RegisterCommands(CommandDispatcher<RuntimeCommandSource> dispatcher) {
			dispatcher.Register(c => c.Literal("setblock")
				.Then(c => c.Argument("x", new CoordinateArgumentType())
					.Then(c => c.Argument("y", new CoordinateArgumentType())
						.Then(c => c.Argument("block", new BlockArgumentType())
							.Executes(ctx => {
								Block block = ctx.GetArgument<Block>("block");
								Vector2 playerPos = ctx.Source.data.Player.GetPos();
								var blockPos = new BlockPos {
									x = Mathf.FloorToInt(ctx.GetArgument<Coordinate>("x").GetPos(playerPos.x)),
									y = Mathf.FloorToInt(ctx.GetArgument<Coordinate>("y").GetPos(playerPos.y))
								};
								ctx.Source.execServices.Level.SetBlockState(blockPos, block.defaultState);
								Logger.LogInfo("Set block {} at {}", block.GetIdentifier(), blockPos);
								return 1;
							})
						)
					)
				)
			);

			dispatcher.Register(c => c.Literal("tp")
				.Then(c => c.Argument("x", new CoordinateArgumentType())
					.Then(c => c.Argument("y", new CoordinateArgumentType())
						.Executes(ctx => Teleport(ctx.Source.data.Player.GetGuid(), ctx))
					)
				).Then(c => c.Argument("target", new GuidArgumentType())
					.Then(c => c.Argument("x", new CoordinateArgumentType())
						.Then(c => c.Argument("y", new CoordinateArgumentType())
							.Executes(ctx => Teleport(ctx.GetArgument<Guid>("target"), ctx))
						)
					)
				)
			);
			static int Teleport(Guid guid, CommandContext<RuntimeCommandSource> ctx) {
				if (!ctx.Source.data.Entities.TryGetEntity(guid, out IEntityView target)) {
					return 0;
				}

				Vector2 pos = target.GetPos();
				float x = ctx.GetArgument<Coordinate>("x").GetPos(pos.x);
				float y = ctx.GetArgument<Coordinate>("y").GetPos(pos.y);

				ctx.Source.execServices.Entity.SetPos(target.GetGuid(), new Vector2(x, y));
				Logger.LogInfo("teleported {} to x:{} y:{}", target, x, y);

				return 1;
			};

			dispatcher.Register(c => c.Literal("spawn")
				.Then(c => c.Argument("entityType", new EntityDescriptorArgumentType())
					.Executes(ctx => SpawnEntity(false, ctx))
					.Then(c => c.Argument("x", new CoordinateArgumentType())
						.Then(c => c.Argument("y", new CoordinateArgumentType())
							.Executes(ctx => SpawnEntity(true, ctx))
						)
					)
				)
			);
			static int SpawnEntity(bool hasPos, CommandContext<RuntimeCommandSource> ctx) {
				EntityDescriptor entityDescriptor = ctx.GetArgument<EntityDescriptor>("entityType");
				Vector2 pos = ctx.Source.data.Player.GetPos();

				if (hasPos) {
					Coordinate x = ctx.GetArgument<Coordinate>("x");
					Coordinate y = ctx.GetArgument<Coordinate>("y");
				}

				ctx.Source.execServices.Level.SpawnEntity(entityDescriptor, pos);
				return 1;
			}

			dispatcher.Register(c => c.Literal("give")
				.Then(c => c.Argument("item", new ItemArgumentType())
					.Executes(ctx => GiveItem(1, ctx))
					.Then(c => c.Argument("quantity", Arguments.Integer(min: 1))
						.Executes(ctx => GiveItem(ctx.GetArgument<int>("quantity"), ctx))
					)
				)
			);
			static int GiveItem(int quantity, CommandContext<RuntimeCommandSource> ctx) {
				Item item = ctx.GetArgument<Item>("item");

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
					ctx.Source.execServices.Player.TryAddItemStack(stacks[j]);
				}

				return 1;
			}

		}

	}
}
