using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.ItemSystem.Container;
using SoulboundEngine.Client.Players;
using SoulboundEngine.Client.World;
using SoulboundEngine.Client.World.EntitySystem;
using SoulboundEngine.Client.World.LevelDomain;
using System;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.Runtime.Services {
	public class RuntimeExecutionServices : IRuntimeExecutionServices {
		private IPlayerExecutionService? _player;
		private IEntityExecutionService? _entity;
		private ILevelExecutionService? _level;

		public IPlayerExecutionService Player {
			get => _player ?? throw new InvalidOperationException("Runtime player execution only available within world session");
		}
		public IEntityExecutionService Entity {
			get => _entity ?? throw new InvalidOperationException("Runtime entity execution only available within world session");
		}
		public ILevelExecutionService Level {
			get => _level ?? throw new InvalidOperationException("Runtime level execution only available within world session");
		}

		public void SetWorldSessionState(WorldSession session) {
			_player = new RuntimePlayerExecutionService(session.player);
			_entity = new RuntimeEntityExecutionService(session.level);
			_level = session.level;
		}

		public void ExitWorldSessionState() {
			_player = null;
			_entity = null;
			_level = null;
		}
	}

	public class RuntimePlayerExecutionService : IPlayerExecutionService {
		public readonly Player player;
		private readonly IInventoryExecutionService _inventory;
		public IInventoryExecutionService Inventory => _inventory;

		public RuntimePlayerExecutionService(Player player) {
			this.player = player;
			_inventory = new RuntimeInventoryExecutionService(player.GetInventory());
		}

		public void SetPos(Vector2 pos) => player.SetPos(pos);

		public bool TryAddItemStack(ItemStack itemStack) => player.TryAddItemStack(itemStack);
	}

	public class RuntimeInventoryExecutionService : IInventoryExecutionService {
		private readonly Inventory inventory;

		public RuntimeInventoryExecutionService(Inventory inventory) {
			this.inventory = inventory;
		}

		public void SetStack(int slotIndex, ItemStack? stack) {
			inventory.GetSlot(slotIndex).SetStack(stack);
		}
	}

	public class RuntimeEntityExecutionService : IEntityExecutionService {
		public readonly IEntityManager entityManager;

		public RuntimeEntityExecutionService(IEntityManager entityManager) {
			this.entityManager = entityManager;
		}

		public void AddEntity(Entity entity) {
			entityManager.AddEntity(entity);
		}

		public void SetPos(Guid entityGuid, Vector2 pos) {
			if (entityManager.TryGetEntity(entityGuid, out Entity entity)) {
				entity.SetPos(pos);
			}
		}
	}
}
