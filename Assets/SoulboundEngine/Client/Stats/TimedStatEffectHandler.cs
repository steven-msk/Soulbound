using SoulboundEngine.Client.ItemSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SoulboundEngine.Client.Stats {
	[Obsolete]
	public class TimedStatEffectHandler : NonPersistentStatEffectHandler {
		private IStatProvider provider;
		private float durationSeconds;
		private bool resetOnEnable;
		private (Coroutine coroutine, IStatReceiver receiver)? active;

		public TimedStatEffectHandler(IStatProvider provider, IEnumerable<AbstractValueModifier> usedStats, float durationSeconds, bool resetOnEnable)
			: base(usedStats) {
			this.provider = provider;
			this.durationSeconds = durationSeconds;
			this.resetOnEnable = resetOnEnable;
		}

		public override void Enable(IStatReceiver receiver) {
			if (IsActive() && resetOnEnable) {
				this.ResetTimer();
			} else {
				receiver.ApplyStats(usedStats, provider);
				active = StartTimer(receiver);
			}
		}

		public override void Disable(IStatReceiver receiver) {
			if (active != null) {
				CoroutineRunner.instance.StopCoroutine(active.Value.coroutine);
				active = null;
			}
			receiver.RevokeStats(usedStats, provider);
		}

		public void ResetTimer() {
			if (active != null) {
				CoroutineRunner.instance.StopCoroutine(active.Value.coroutine);
				active = StartTimer(active.Value.receiver);
			}
		}

		private (Coroutine, IStatReceiver) StartTimer(IStatReceiver receiver) {
			return (CoroutineRunner.instance.StartCoroutine(Countdown(receiver)), receiver);
		}

		public bool IsActive() => active != null;

		private IEnumerator Countdown(IStatReceiver receiver) {
			yield return new WaitForSecondsRealtime(durationSeconds);
			receiver.RevokeStats(usedStats, provider);
			active = null;
		}
	}
}
