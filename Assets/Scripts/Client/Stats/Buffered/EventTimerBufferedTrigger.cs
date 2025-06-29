using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class EventTimerBufferedTrigger : IBufferedTrigger {
	[SerializeField] private string eventID;
	[SerializeField] private float waitTime;
	[SerializeField] private BufferedTriggerCondition condition;
	private Coroutine currentCoroutine = null;

	public BufferedTriggerState State { get; set; }

	public Func<bool> InvocationValidator => condition.ToValidator();

	public void Enable(BufferedStat stat, IStatProvider source) {
		EventBus<GameEvent>.Subscribe(GameEvent.FromID(eventID), this.CoroutineInvoker(stat, source));
	}

	public void Disable(BufferedStat stat, IStatProvider source) {
		EventBus<GameEvent>.Unsubscribe(GameEvent.FromID(eventID), this.CoroutineInvoker(stat, source));
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

	private Action CoroutineInvoker(BufferedStat stat, IStatProvider source) {
		return () => currentCoroutine = CoroutineRunner.instance.StartCoroutine(this.DelayedInvoke(State.GetInvokeAction(this, stat, source)));
	}
}
