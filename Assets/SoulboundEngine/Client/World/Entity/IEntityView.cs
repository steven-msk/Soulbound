using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundEngine.Client.World.EntitySystem {
	public interface IEntityView {
		Guid GetGuid();
		string GetID();
		Vector2 GetPos();
	}
}
