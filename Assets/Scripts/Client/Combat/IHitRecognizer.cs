using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.Combat {
	public interface IHitRecognizer {
		bool ShouldRegisterHit(Collider2D other);
		void OnHitFrame(Collider2D other);
	}
}
