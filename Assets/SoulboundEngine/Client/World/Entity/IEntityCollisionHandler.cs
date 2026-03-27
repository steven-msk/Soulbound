using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundEngine.Client.World.EntitySystem {
	public interface IEntityCollisionHandler {
		void OnCollisionEnter(EntityCollision collision);
		void OnCollisionExit(EntityCollision collision);
	}
}
