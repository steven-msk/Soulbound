using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

[Obsolete]
public class TimerBufferedTrigger : IBufferedTrigger {
	[JsonProperty] private readonly float waitTimeSeconds;
	[JsonProperty] private readonly BufferedTriggerCondition condition;
	private Coroutine currentCoroutine = null;

	public TimerBufferedTrigger(float waitTimeSeconds, BufferedTriggerCondition condition) {
		this.waitTimeSeconds = waitTimeSeconds;
		this.condition = condition;
	}

	[JsonIgnore] public Func<bool> InvocationValidator => condition.ToValidator();

	public void Enable(IBufferedStatImpl stat, IStatProvider source, BufferedTriggerState state) {
		InvocationHelper.If(ValidateExecution(stat, source, false), () => {
			currentCoroutine = CoroutineRunner.instance.StartCoroutine(this.DelayedInvoke(state.GetInvokeAction(this, stat, source)));
		});
		Debug.Log("timer buffer enabled");
	}

	public void Disable(IBufferedStatImpl stat, IStatProvider source, BufferedTriggerState state) {
		InvocationHelper.If(currentCoroutine != null && ValidateExecution(stat, source, false), () => {
			CoroutineRunner.instance.StopCoroutine(currentCoroutine);
			currentCoroutine = null;
		});
		Debug.Log("timer buffer disabled");
	}

	public void Invoke(IBufferedStatImpl stat, Action action) => InvocationHelper.If(InvocationValidator.Invoke(), action);

	private IEnumerator DelayedInvoke(Action invokeAction) {
		yield return new WaitForSeconds(waitTimeSeconds);
		Debug.Log("timer buffer invoked");
		invokeAction.Invoke();
	}

	public bool ValidateExecution(IBufferedStatImpl stat, IStatProvider source, bool log) {
		InvocationHelper.If(log && waitTimeSeconds == 0, () => {
			UnityEngine.Debug.LogWarning($"WaitTime field of TimerBufferedTrigger in {stat.GetStatDefinition()} @ {source} is set to 0. " +
				$"This might be an intentional value, but in most cases indicates a broken trigger behavior");
		});
		return true;
	}
}
