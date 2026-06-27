using SoulboundEngine.Client.Item.Container;
using SoulboundEngine.Client.World.Entity;
using SoulboundEngine.Client.World.Level;
using SoulboundEngine.Core.Registry;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.Runtime.Services {
	using PlayerEntity = Player.PlayerEntity;

	public sealed class RuntimeDataProvider : IRuntimeDataProvider {
		private IRuntimePlayerDataProvider? _player;
		private IRuntimeEntityDataProvider? _entities;

		public IRuntimePlayerDataProvider Player {
			get => this._player ?? throw new InvalidOperationException("Runtime player data is only available within a world session");
		}
		public IRuntimeEntityDataProvider Entities {
			get => this._entities ?? throw new InvalidOperationException("Runtime entity data is only available within a world session");
		}

		public void SetWorldSessionState(WorldSession session) {
			this._player = new RuntimePlayerDataProvider(session.player);
			this._entities = new RuntimeEntityDataProvider(session.level);
		}

		public void ExitWorldSessionState() {
			this._player = null;
			this._entities = null;
		}
	}

	public class RuntimePlayerDataProvider : IRuntimePlayerDataProvider {
		public readonly PlayerEntity player;

		public RuntimePlayerDataProvider(PlayerEntity player) {
			this.player = player;
		}

		public Guid GetGuid() => this.player.guid;

		public Identifier GetIdentifier() => EntityDescriptor.GetIdentifier(this.player.GetDescriptor());

		public Vector2 GetPos() => this.player.GetPosition();

		public InventoryData GetInventory() {
			IInventory inventory = this.player.GetInventory();
			IEnumerable<int> slots = inventory.GetAllSlots();

			return new InventoryData {
				slots = slots,
				stacks = slots.ToDictionary(s => s, s => inventory.GetSlot(s).GetStack())
			};
		}

	}


	public class RuntimeEntityDataProvider : IRuntimeEntityDataProvider {
		public readonly IEntityManager entityManager;

		public RuntimeEntityDataProvider(IEntityManager entityManager) {
			this.entityManager = entityManager;
		}

		public IEnumerable<IEntityView> GetAllEntities() {
			foreach (var entity in this.entityManager.GetAllEntities()) {
				yield return new EntityView(entity);
			}
		}

		public bool TryGetEntity(Guid guid, out IEntityView entity) {
			bool found = this.entityManager.TryGetEntity(guid, out Entity result);
			entity = found
				? new EntityView(result)
				: default;
			return found;
		}

		private readonly struct EntityView : IEntityView {
			private readonly Entity entity;

			public EntityView(Entity entity) {
				this.entity = entity;
			}

			public Guid GetGuid() => this.entity.guid;

			public Identifier GetIdentifier() => EntityDescriptor.GetIdentifier(this.entity.GetDescriptor());

			public Vector2 GetPos() => this.entity.GetPosition();

			public override string ToString() {
				return $"entity:{this.GetIdentifier()}/{this.GetGuid()}";
			}
		}
	}
}
