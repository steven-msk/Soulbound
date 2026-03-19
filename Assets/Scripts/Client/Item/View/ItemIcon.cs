using SoulboundBackend.Core.Assets;
using System;
using UnityEngine;

namespace SoulboundBackend.Client.ItemSystem.View {
	public record ItemIcon {
		public readonly AssetKey spriteKey;
		public readonly float intendedPixelsPerUnit;

		public ItemIcon(AssetKey spriteKey, float pixelsPerUnit) {
			this.spriteKey = spriteKey;
			this.intendedPixelsPerUnit = pixelsPerUnit;
		}
	}
}
