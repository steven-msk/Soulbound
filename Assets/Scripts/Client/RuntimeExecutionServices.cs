using SoulboundBackend.Client.World;
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

		public IPlayerExecutionService Player {
			get => _player ?? throw new InvalidOperationException("Runtime player execution only available within world session");
		}

		public void SetWorldSesstionState(WorldSession session) {
			_player = new RuntimePlayerExecutionService(session.player);
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
}
