using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public sealed class StatEffectHandler_test : NonPersistentStatEffectHandler {
	private IStatProvider provider;
	private Coroutine runningCoroutine = null;

	public StatEffectHandler_test(IStatProvider provider, IEnumerable<AbstractSerializableStat> usedStats)
		: base(usedStats) {
		this.provider = provider;
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
		yield return new WaitForSecondsRealtime(5);
		source.RevokeStats(usedStats, provider);
		runningCoroutine = null;
	}
}
