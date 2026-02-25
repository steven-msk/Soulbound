using Assets.Scripts.Core.Debug.Command;
using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable

namespace SoulboundBackend.Client {
	public sealed class RuntimeDataProvider : IRuntimeDataProvider {
		private IRuntimePlayerDataProvider? _player;
		private IRuntimeEntityDataProvider? _entities;

		public IRuntimePlayerDataProvider Player {
			get => _player ?? throw new InvalidOperationException("Runtime player data is only available within a world session");
		}
		public IRuntimeEntityDataProvider Entities {
			get => _entities ?? throw new InvalidOperationException("Runtime entity data is only available within a world session");
		}

		public void SetWorldSessionState(WorldSession session) {
			_player = new RuntimePlayerDataProvider(session.player);
			_entities = new RuntimeEntityDataProvider(session.entityManager);
		}

		public void ExitWorldSessionState() {
			_player = null;
			_entities = null;
		}
	}

	public class RuntimePlayerDataProvider : IRuntimePlayerDataProvider {
		public readonly PlayerController player;

		public RuntimePlayerDataProvider(PlayerController player) {
			this.player = player;
		}

		public Guid GetGuid() => player.id;

		public string GetName() => "player";

		public Vector2 GetPos() => player.position;
	}


	public class RuntimeEntityDataProvider : IRuntimeEntityDataProvider {
		public readonly EntityManager entityManager;

		public RuntimeEntityDataProvider(EntityManager entityManager) {
			this.entityManager = entityManager;
		}

		public IEnumerable<IEntityView> GetAllEntities() {
			foreach (var guid in entityManager.GetAllEntities()) {
				entityManager.GetEntityByID(guid, out Entity entity);
				yield return new EntityView(entity);
			}
		}

		public bool TryGetEntity(Guid guid, out IEntityView entity) {
			bool found = entityManager.GetEntityByID(guid, out Entity result);
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

			public Guid GetGuid() => entity.id;

			public string GetName() => entity.name;

			public Vector2 GetPos() => entity.position;

			public override string ToString() {
				return $"entity:{GetName()}/{GetGuid()}";
			}
		}
	}
}
