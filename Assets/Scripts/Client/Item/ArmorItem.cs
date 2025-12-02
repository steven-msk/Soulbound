using SoulboundBackend.Client.Stats;
using System;
using UnityEngine;

#nullable enable

namespace SoulboundBackend.Client.ItemSystem {
	public abstract class ArmorItem : StatItem {
		public event Action<IStatReceiver>? onEquipped;
		public event Action<IStatReceiver>? onUnequipped;
		public abstract ArmorType armorType { get; }

		public void OnEquip(IStatReceiver receiver) {
			onEquipped?.Invoke(receiver);
			//(this as IStatProvider).BeginActivatorContexts(receiver);
			UnityEngine.Debug.Log("equipped");
		}

		public void OnUnequipped(IStatReceiver receiver) {
			onUnequipped?.Invoke(receiver);
			//(this as IStatProvider).EndActivatorContexts(receiver);
			UnityEngine.Debug.Log("unequipped");
		}
	}
}