using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.Combat {
	public interface IHitRecognizer {
		bool ShouldRegisterHit(Hurtbox other);
		void OnHitFrame(Hurtbox other);
	}
}
