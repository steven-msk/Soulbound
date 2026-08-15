using SoulboundEngine.Item;
using SoulboundEngine.Item.Container;
using SoulboundEngine.World.Entity;
using SoulboundEngine.World.Level;
using SoulboundEngine.World.Player;
using System;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.World.Services {
	using Entity = Entity.Entity;

	public class RuntimeExecutionServices : IRuntimeExecutionServices {
		private IPlayerExecutionService? _player;
		private IEntityExecutionService? _entity;
		private ILevelExecutionService? _level;

		public IPlayerExecutionService Player {
			get => this._player ?? throw new InvalidOperationException("Runtime player execution only available within world session");
		}
		public IEntityExecutionService Entity {
			get => this._entity ?? throw new InvalidOperationException("Runtime entity execution only available within world session");
		}
		public ILevelExecutionService Level {
			get => this._level ?? throw new InvalidOperationException("Runtime level execution only available within world session");
		}

		public void SetWorldSessionState(WorldSession session, PlayerEntity player) {
			this._player = new RuntimePlayerExecutionService(player);
			this._entity = new RuntimeEntityExecutionService(session.level);
			this._level = session.level;
		}

		public void ExitWorldSessionState() {
			this._player = null;
			this._entity = null;
			this._level = null;
		}
	}

	public class RuntimePlayerExecutionService : IPlayerExecutionService {
		public readonly PlayerEntity player;
		private readonly IInventoryExecutionService _inventory;
		public IInventoryExecutionService Inventory => this._inventory;

		public RuntimePlayerExecutionService(PlayerEntity player) {
			this.player = player;
			this._inventory = new RuntimeInventoryExecutionService(player.GetInventory());
		}

		public void SetPos(Vector2 pos) => this.player.SetPosition(pos);

		public bool TryAddItemStack(ItemStack itemStack) => this.player.TryAddItemStack(itemStack);
	}

	public class RuntimeInventoryExecutionService : IInventoryExecutionService {
		private readonly IInventory inventory;

		public RuntimeInventoryExecutionService(IInventory inventory) {
			this.inventory = inventory;
		}

		public void SetStack(int slotIndex, ItemStack stack) {
			this.inventory.GetSlot(slotIndex).SetStack(stack);
		}
	}

	public class RuntimeEntityExecutionService : IEntityExecutionService {
		public readonly IEntityManager entityManager;

		public RuntimeEntityExecutionService(IEntityManager entityManager) {
			this.entityManager = entityManager;
		}

		public void AddEntity(Entity entity) {
			this.entityManager.AddEntity(entity);
		}

		public void SetPos(Guid entityGuid, Vector2 pos) {
			if (this.entityManager.TryGetEntity(entityGuid, out Entity entity)) {
				entity.SetPosition(pos);
			}
		}
	}
}
