using SoulboundBackend.Core.AssetManagement;
using System;
using UnityEngine;

namespace SoulboundBackend.Client.ItemSystem {
	public record ItemIcon {
		public readonly AssetKey spriteKey;
		public readonly float intendedPixelsPerUnit;

		public ItemIcon(AssetKey spriteKey, float pixelsPerUnit) {
			this.spriteKey = spriteKey;
			this.intendedPixelsPerUnit = pixelsPerUnit;
		}
	}
}
