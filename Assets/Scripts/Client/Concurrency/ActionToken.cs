using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

namespace SoulboundBackend.Client.Concurrency {
	public readonly struct ActionToken {
		public readonly int id;
		public readonly int effectivePriority;

		public ActionToken(int id, int effectivePriority) { 
			this.id = id;
			this.effectivePriority = effectivePriority;
		}

		public ActionToken(int id)
			: this(id, 0) {
		}
	}
}
