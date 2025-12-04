using SoulboundBackend.Client.World.EntitySystem.SpawnData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.EntitySystem {
	public interface IEntitySpawnable<TData> where TData : ISpawnData {
		void ApplySpawnData(TData spawnData);
	}
}
