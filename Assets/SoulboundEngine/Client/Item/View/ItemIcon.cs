using SoulboundEngine.Core.Assets;
using System;
using UnityEngine;

namespace SoulboundEngine.Client.ItemSystem.View {
	public record ItemIcon {
		public readonly AssetKey spriteKey;
		public readonly float intendedPixelsPerUnit;

		public ItemIcon(AssetKey spriteKey, float pixelsPerUnit) {
			this.spriteKey = spriteKey;
			this.intendedPixelsPerUnit = pixelsPerUnit;
		}
	}
}
