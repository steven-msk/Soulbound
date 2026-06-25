using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Client.Player;
using System;
using System.Collections.Generic;

namespace SoulboundEngine.Client.UI.Screen {
	public class InventoryScreens {
		private static readonly Dictionary<InventoryScreenHandlerType, IProviderBase> providers = new();

		public InventoryScreens() {
			Register(InventoryScreenHandlerType.DEFAULT_INVENTORY, IProvider<DefaultInventoryScreenHandler, DefaultInventoryScreen>.Of(
				(handler, playerInventory) => new DefaultInventoryScreen(handler, playerInventory)
			));
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

		public static IScreenHandle Open(InventoryScreenHandler handler, SoulboundClient client, PlayerInventory playerInventory) {
			try {
				return client.OpenScreen(GetProvider(handler.GetHandlerType()).Create(handler, playerInventory));
			} catch (Exception e) {
				Logger.LogFatal(e);
				throw;
			}
		}

		public static IScreenHandle Open<THandler>(InventoryScreenHandlerType<THandler> type, SoulboundClient client, PlayerInventory playerInventory) where THandler : InventoryScreenHandler {
			try {
				return client.OpenScreen(GetProvider(type).Create(type.Create(), playerInventory));
			} catch (Exception e) {
				Logger.LogFatal(e);
				throw;
			}
		}

		private interface IProviderBase {
			Screen Create(InventoryScreenHandler handler, PlayerInventory playerInventory);
		}

		private interface IProvider<THandler, TScreen> : IProviderBase where THandler : InventoryScreenHandler where TScreen : Screen, InventoryScreenHandlerProvider<THandler> {
			TScreen Create(THandler handler, PlayerInventory playerInventory);

			Screen IProviderBase.Create(InventoryScreenHandler handler, PlayerInventory playerInventory) => this.Create((THandler)handler, playerInventory);

			public static IProvider<THandler, TScreen> Of(Func<THandler, PlayerInventory, TScreen> func) => new Impl(func);

			private sealed class Impl : IProvider<THandler, TScreen> {
				private Func<THandler, PlayerInventory,  TScreen> func;

				public Impl(Func<THandler, PlayerInventory, TScreen> func) {
					this.func = func;
				}

				public TScreen Create(THandler handler, PlayerInventory playerInventory) => this.func(handler, playerInventory);
			}
		}
	}
}
