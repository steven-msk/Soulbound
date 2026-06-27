using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Client.Player;
using SoulboundEngine.Client.Render.Item;
using SoulboundEngine.Core.Assets;
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.UI.Screen {
	public class InventoryScreens {
		private static readonly Dictionary<InventoryScreenHandlerType, IProviderBase> providers = new();

		public InventoryScreens() {
			Register(InventoryScreenHandlerType.DEFAULT_INVENTORY, IProvider<DefaultInventoryScreenHandler, DefaultInventoryScreen>.Of(
				(ctx) => new DefaultInventoryScreen(ctx, AssetManager.Resolve<VisualTreeAsset>(new AssetKey("DefaultInventoryScreen")))
			));
			Register(InventoryScreenHandlerType.CHEST, IProvider<ChestInventoryScreenHandler, ChestInventoryScreen>.Of(
				(ctx) => new ChestInventoryScreen(ctx, AssetManager.Resolve<VisualTreeAsset>(new AssetKey("ChestInventoryScreen")))
			));
		}

		private static InventoryScreen<THandler>.Context CreateContext<THandler>(THandler handler, PlayerInventory playerInventory, PlayerEntity player, ItemRenderManager itemRenderManager)
				where THandler : InventoryScreenHandler {
			return new InventoryScreen<THandler>.Context {
				handler = handler,
				player = player,
				playerInventory = playerInventory,
				itemRenderManager = itemRenderManager
			};
		}

		private static ProviderContext CreateProviderContext(InventoryScreenHandler handler, PlayerInventory playerInventory, PlayerEntity player, ItemRenderManager itemRenderManager) {
			return new ProviderContext {
				handler = handler,
				itemRenderManager = itemRenderManager,
				player = player,
				playerInventory = playerInventory
			};
		}

		private static InventoryScreen<THandler>.Context FromProvider<THandler>(ProviderContext ctx) where THandler : InventoryScreenHandler {
			return CreateContext((THandler)ctx.handler, ctx.playerInventory, ctx.player, ctx.itemRenderManager);
		}

		private static void Register<THandler, TScreen>(InventoryScreenHandlerType<THandler> type, IProvider<THandler, TScreen> provider)
				where THandler : InventoryScreenHandler where TScreen : Screen, InventoryScreenHandlerProvider<THandler> {
			providers.Add(type, provider);
		}

		private static IProviderBase GetProvider<THandler>(InventoryScreenHandlerType<THandler> type)
				where THandler : InventoryScreenHandler {
			return providers[type];
		}

		private static IProviderBase GetProvider(InventoryScreenHandlerType type) => providers[type];

		public static IScreenHandle Open(InventoryScreenHandler handler, SoulboundClient client, PlayerInventory playerInventory, PlayerEntity player) {
			try {
				ProviderContext ctx = CreateProviderContext(handler, playerInventory, player, client.ItemRenderManager);
				return client.OpenScreen(GetProvider(handler.GetHandlerType()).Create(ctx));
			} catch (Exception e) {
				Logger.LogFatal(e);
				return null;
			}
		}

		public static IScreenHandle Open<THandler>(InventoryScreenHandlerType<THandler> type, SoulboundClient client, PlayerInventory playerInventory, PlayerEntity player) 
				where THandler : InventoryScreenHandler {
			try {
				ProviderContext ctx = CreateProviderContext(type.Create(playerInventory), playerInventory, player, client.ItemRenderManager);
				return client.OpenScreen(GetProvider(type).Create(ctx));
			} catch (Exception e) {
				Logger.LogFatal(e);
				return null;
			}
		}

		private interface IProviderBase {
			Screen Create(ProviderContext ctx);
		}

		private interface IProvider<THandler, TScreen> : IProviderBase where THandler : InventoryScreenHandler where TScreen : Screen, InventoryScreenHandlerProvider<THandler> {
			TScreen Create(InventoryScreen<THandler>.Context ctx);

			Screen IProviderBase.Create(ProviderContext ctx) {
				return this.Create(FromProvider<THandler>(ctx));
			}

			public static IProvider<THandler, TScreen> Of(Func<InventoryScreen<THandler>.Context, TScreen> func) => new Impl(func);

			private sealed class Impl : IProvider<THandler, TScreen> {
				private readonly Func<InventoryScreen<THandler>.Context, TScreen> func;

				public Impl(Func<InventoryScreen<THandler>.Context, TScreen> func) {
					this.func = func;
				}

				public TScreen Create(InventoryScreen<THandler>.Context ctx) {
					return this.func(ctx);
				}
			}
		}

		private struct ProviderContext {
			public InventoryScreenHandler handler;
			public PlayerInventory playerInventory;
			public PlayerEntity player;
			public ItemRenderManager itemRenderManager;
		}
	}
}
