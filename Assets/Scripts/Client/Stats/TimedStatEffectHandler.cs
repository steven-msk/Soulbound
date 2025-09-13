using SoulboundBackend.Client.ItemSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SoulboundBackend.Client.Stats {
	public class TimedStatEffectHandler : NonPersistentStatEffectHandler {
		private IStatProvider provider;
		private float durationSeconds;
		private Coroutine runningCoroutine = null;

		public TimedStatEffectHandler(IStatProvider provider, IEnumerable<AbstractSerializableStat> usedStats, float durationSeconds)
			: base(usedStats) {
			this.provider = provider;
			this.durationSeconds = durationSeconds;
		}

		public override void Enable(IStatReceiver receiver) {
			receiver.ApplyStats(usedStats, provider);
			runningCoroutine = CoroutineRunner.instance.StartCoroutine(Countdown(receiver));
		}

		public override void Disable(IStatReceiver receiver) {
			if (runningCoroutine != null) {
				CoroutineRunner.instance.StopCoroutine(runningCoroutine);
				runningCoroutine = null;
			}
			receiver.RevokeStats(usedStats, provider);
		}

		private IEnumerator Countdown(IStatReceiver receiver) {
			yield return new WaitForSecondsRealtime(durationSeconds);
			receiver.RevokeStats(usedStats, provider);
			runningCoroutine = null;
		}
	}
}
