using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.EntitySystem;
using SoulboundBackend.Core.Debug.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable

namespace SoulboundBackend.Client {
	public class RuntimeExecutionServices : IRuntimeExecutionServices {
		private IPlayerExecutionService? _player;
		private IEntityExecutionService? _entity;

		public IPlayerExecutionService Player {
			get => _player ?? throw new InvalidOperationException("Runtime player execution only available within world session");
		}
		public IEntityExecutionService Entity {
			get => _entity ?? throw new InvalidOperationException("Runtime entity execution only available within world session");
		}

		public void SetWorldSesstionState(WorldSession session) {
			_player = new RuntimePlayerExecutionService(session.player);
			_entity = new RuntimeEntityExecutionService(session.entityManager);
		}

		public void ExitWorldSessionState() {
			_player = null;
		}
	}

	public class RuntimePlayerExecutionService : IPlayerExecutionService {
		public readonly PlayerController player;

		public RuntimePlayerExecutionService(PlayerController player) {
			this.player = player;
		}

		public void SetPos(Vector2 pos) => player.position = pos;
	}

	public class RuntimeEntityExecutionService : IEntityExecutionService {
		public readonly EntityManager entityManager;

		public RuntimeEntityExecutionService(EntityManager entityManager) {
			this.entityManager = entityManager;
		}

		public void SetPos(Guid entityGuid, Vector2 pos) {
			if (entityManager.GetEntityByID(entityGuid, out Entity entity)) {
				entity.position = pos;
			}
		}
	}
}
