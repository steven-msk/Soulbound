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
			_entities = new RuntimeEntityDataProvider(session.level);
		}

		public void ExitWorldSessionState() {
			_player = null;
			_entities = null;
		}
	}

	public class RuntimePlayerDataProvider : IRuntimePlayerDataProvider {
		public readonly Player player;

		public RuntimePlayerDataProvider(Player player) {
			this.player = player;
		}

		public Guid GetGuid() => player.guid;

		public string GetID() => "player";

		public Vector2 GetPos() => player.GetPos();
	}


	public class RuntimeEntityDataProvider : IRuntimeEntityDataProvider {
		public readonly IEntityManager entityManager;

		public RuntimeEntityDataProvider(IEntityManager entityManager) {
			this.entityManager = entityManager;
		}

		public IEnumerable<IEntityView> GetAllEntities() {
			foreach (var entity in entityManager.GetAllEntities()) {
				yield return new EntityView(entity);
			}
		}

		public bool TryGetEntity(Guid guid, out IEntityView entity) {
			bool found = entityManager.TryGetEntity(guid, out Entity result);
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

			public Guid GetGuid() => entity.guid;

			public string GetID() => entity.descriptor.id;

			public Vector2 GetPos() => entity.GetPos();

			public override string ToString() {
				return $"entity:{GetID()}/{GetGuid()}";
			}
		}
	}
}
