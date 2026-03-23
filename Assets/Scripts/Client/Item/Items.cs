using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using SoulboundBackend.Common.Patterns;
using SoulboundBackend.Core;

namespace SoulboundBackend.Client.ItemSystem {
	public partial class Items {
		// contract initialization, do not modify
		private static readonly IRegistrationContract<Item, IRegistrationKey<Item>> _contract = Registry<Item>.SetContract(new ItemRegistrationContract());

		public static readonly StackItem stackitem_1 = Registry<Item>.Add(new StackItem(1));
		public static readonly StackItem stackitem_10 = Registry<Item>.Add(new StackItem(10));
		public static readonly StackItem stackitem_256 = Registry<Item>.Add(new StackItem(Item.DEFAULT_FULL_STACK));
		public static readonly StackItem stackitem_64 = Registry<Item>.Add(new StackItem(64));
		public static readonly PlaceableItem placeableItem = Registry<Item>.Add(new PlaceableItem());
		public static readonly TeleportPlayerItem teleportPlayerItem = Registry<Item>.Add(new TeleportPlayerItem());
		public static readonly SpawnEntityItem spawnEntityItem = Registry<Item>.Add(new SpawnEntityItem());
		public static readonly ChargeableItem chargeableItem = Registry<Item>.Add(new ChargeableItem());
		public static readonly DebugPointerItem debugPointer = Registry<Item>.Add(new DebugPointerItem());
		public static readonly InventoryListenerItem inventoryListenerItem = Registry<Item>.Add(new InventoryListenerItem());
		public static readonly BlockBreakerItem blockBreakerItem = Registry<Item>.Add(new BlockBreakerItem());

		public sealed class ItemRegistrationContract : IRegistrationContract<Item, IRegistrationKey<Item>> {
			public IRegistrationKey<Item> ValueToKey(Item value) {
				return new Item.RegistrationKey(value.GetID());
			}
		}
	}
}
