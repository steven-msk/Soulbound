using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class TimedStatEffectHandler : NonPersistentStatEffectHandler {
	private IStatProvider provider;
	private float durationSeconds;
	private Coroutine runningCoroutine = null;

	public TimedStatEffectHandler(IStatProvider provider, IEnumerable<AbstractSerializableStat> usedStats, float durationSeconds)
		: base(usedStats) {
		this.provider = provider;
		this.durationSeconds = durationSeconds;
	}

	public override void Enable(IStatSource source) {
		source.ApplyStats(usedStats, provider);
		runningCoroutine = CoroutineRunner.instance.StartCoroutine(Countdown(source));
	}

	public override void Disable(IStatSource source) {
		if (runningCoroutine != null) {
			CoroutineRunner.instance.StopCoroutine(runningCoroutine);
			runningCoroutine = null;
		}
		source.RevokeStats(usedStats, provider);
	}

	private IEnumerator Countdown(IStatSource source) {
		yield return new WaitForSecondsRealtime(durationSeconds);
		source.RevokeStats(usedStats, provider);
		runningCoroutine = null;
	}
}
