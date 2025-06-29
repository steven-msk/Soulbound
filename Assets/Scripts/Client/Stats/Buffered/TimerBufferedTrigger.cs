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

	public void Enable(BufferedStat stat, IStatProvider source) {
		currentCoroutine = CoroutineRunner.instance.StartCoroutine(this.DelayedInvoke(State.GetInvokeAction(this, stat, source)));
	}

	public void Disable(BufferedStat stat, IStatProvider source) {
		if (currentCoroutine != null) {
			CoroutineRunner.instance.StopCoroutine(currentCoroutine);
			currentCoroutine = null;
		}
	}

	public void Invoke(BufferedStat stat, Action action) => InvocationHelper.If(InvocationValidator.Invoke(), action);

	private IEnumerator DelayedInvoke(Action invokeAction) {
		yield return new WaitForSeconds(waitTime);
		invokeAction.Invoke();
	}
}
