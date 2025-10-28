using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.Combat {
	public class ContinuousHitRecognizer : IHitRecognizer {
		public void OnHitFrame(Hurtbox other) {
		}

		public bool ShouldRegisterHit(Hurtbox other) {
			return true;
		}
	}
}
