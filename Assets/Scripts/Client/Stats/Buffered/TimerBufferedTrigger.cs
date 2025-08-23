using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class TimerBufferedTrigger : IBufferedTrigger {
	[SerializeField] private float waitTime;
	[SerializeField] private BufferedTriggerCondition condition;
	private Coroutine currentCoroutine = null;

	public BufferedTriggerState State { get; set; }

	public Func<bool> InvocationValidator => condition.ToValidator();

	public void Enable(IBufferedStatImpl stat, IStatProvider source) {
		InvocationHelper.If(ValidateExecution(stat, source, false), () => {
			currentCoroutine = CoroutineRunner.instance.StartCoroutine(this.DelayedInvoke(State.GetInvokeAction(this, stat, source)));
		});
	}

	public void Disable(IBufferedStatImpl stat, IStatProvider source) {
		InvocationHelper.If(currentCoroutine != null && ValidateExecution(stat, source, false), () => {
			CoroutineRunner.instance.StopCoroutine(currentCoroutine);
			currentCoroutine = null;
		});
	}

	public void Invoke(IBufferedStatImpl stat, Action action) => InvocationHelper.If(InvocationValidator.Invoke(), action);

	private IEnumerator DelayedInvoke(Action invokeAction) {
		yield return new WaitForSeconds(waitTime);
		invokeAction.Invoke();
	}

	public bool ValidateExecution(IBufferedStatImpl stat, IStatProvider source, bool log) {
		InvocationHelper.If(log && waitTime == 0, () => {
			UnityEngine.Debug.LogWarning($"WaitTime field of TimerBufferedTrigger in {stat.GetStatType()} @ {source} is set to 0. " +
				$"This might be an intentional value, but in most cases indicates a broken trigger behavior");
		});
		return true;
	}
}
