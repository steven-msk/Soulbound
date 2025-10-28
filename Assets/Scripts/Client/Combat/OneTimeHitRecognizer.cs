using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.Combat {
	public sealed class OneTimeHitRecognizer : IHitRecognizer {
		private bool hasHit = false;

		public void OnHitFrame(Hurtbox other) {
			hasHit = true;
		}

		public bool ShouldRegisterHit(Hurtbox other) {
			return !hasHit;
		}
	}
}
