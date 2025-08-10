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
		InvocationHelper.If(ValidateExecution(stat, source, false), () => {
			EventBus<GameEvent>.Subscribe(GameEvent.FromID(eventID), this.CoroutineInvoker(stat, source));
		});
	}

	public void Disable(BufferedStat stat, IStatProvider source) {
		InvocationHelper.If(ValidateExecution(stat, source, false), () => {
			EventBus<GameEvent>.Unsubscribe(GameEvent.FromID(eventID), this.CoroutineInvoker(stat, source));
			if (currentCoroutine != null) {
				CoroutineRunner.instance.StopCoroutine(currentCoroutine); 
				currentCoroutine = null;
			}
		});
	}

	public void Invoke(BufferedStat stat, Action action) => InvocationHelper.If(InvocationValidator.Invoke(), action);

	private IEnumerator DelayedInvoke(Action invokeAction) {
		yield return new WaitForSeconds(waitTime);
		invokeAction.Invoke();
	}

	private Action CoroutineInvoker(BufferedStat stat, IStatProvider source) {
		return () => currentCoroutine = CoroutineRunner.instance.StartCoroutine(this.DelayedInvoke(State.GetInvokeAction(this, stat, source)));
	}

	public bool ValidateExecution(BufferedStat stat, IStatProvider source, bool log) {
		bool valid = true;
		InvocationHelper.If(log && waitTime == 0, () => {
			UnityEngine.Debug.LogWarning($"WaitTime field of EventTimerBufferedTrigger in {stat.SerializedReference} @ {source} is set to 0. " +
				$"This might be an intentional value, but in most cases indicates a broken trigger behavior");
		});
		if (string.IsNullOrEmpty(eventID)) {
			InvocationHelper.If(log, () => UnityEngine.Debug.LogError($"Null or empty eventID for EventTimerBufferedTrigger in {stat.SerializedReference} @ {source}"));
			valid = false;
		}
		if (GameEvent.FromID(eventID) == null && valid) {
			InvocationHelper.If(log, () => UnityEngine.Debug.LogError($"Invalid eventID: {eventID} for EventTimerBufferedTrigger in {stat.SerializedReference} @ {source}"));
			valid = false; 
		}
		return valid;
	}
}
