using System;
using System.Collections;
using Unity.Plastic.Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

public class EventTimerBufferedTrigger : IBufferedTrigger {
	[JsonProperty] private readonly string eventID;
	[JsonProperty] private readonly float waitTime;
	[JsonProperty] private readonly BufferedTriggerCondition condition;
	private Coroutine currentCoroutine = null;

	public EventTimerBufferedTrigger(string eventID, float waitTime, BufferedTriggerCondition condition) {
		this.eventID = eventID;
		this.waitTime = waitTime;
		this.condition = condition;
	}

	[JsonIgnore] public Func<bool> InvocationValidator => condition.ToValidator();

	public void Enable(IBufferedStatImpl stat, IStatProvider provider, BufferedTriggerState state) {
		InvocationHelper.If(ValidateExecution(stat, provider, false), () => {
			EventBus<GameEvent>.Subscribe(GameEvent.FromID(eventID), this.CoroutineInvoker(stat, provider, state));
		});
	}

	public void Disable(IBufferedStatImpl stat, IStatProvider provider, BufferedTriggerState state) {
		InvocationHelper.If(ValidateExecution(stat, provider, false), () => {
			EventBus<GameEvent>.Unsubscribe(GameEvent.FromID(eventID), this.CoroutineInvoker(stat, provider, state));
			if (currentCoroutine != null) {
				CoroutineRunner.instance.StopCoroutine(currentCoroutine); 
				currentCoroutine = null;
			}
		});
	}

	public void Invoke(IBufferedStatImpl stat, Action action) => InvocationHelper.If(InvocationValidator.Invoke(), action);

	private IEnumerator DelayedInvoke(Action invokeAction) {
		yield return new WaitForSeconds(waitTime);
		invokeAction.Invoke();
	}

	private Action CoroutineInvoker(IBufferedStatImpl stat, IStatProvider provider, BufferedTriggerState state) {
		return null;
		//return () => currentCoroutine = CoroutineRunner.instance.StartCoroutine(this.DelayedInvoke(state.GetInvokeAction(this, stat, provider)));
	}

	public bool ValidateExecution(IBufferedStatImpl stat, IStatProvider provider, bool log) {
		bool valid = true;
		InvocationHelper.If(log && waitTime == 0, () => {
			UnityEngine.Debug.LogWarning($"WaitTime field of EventTimerBufferedTrigger in {stat.GetStatDefinition()} @ {provider} is set to 0. " +
				$"This might be an intentional value, but in most cases indicates a broken trigger behavior");
		});
		if (string.IsNullOrEmpty(eventID)) {
			InvocationHelper.If(log, () => UnityEngine.Debug.LogError($"Null or empty eventID for EventTimerBufferedTrigger in {stat.GetStatDefinition()} @ {provider}"));
			valid = false;
		}
		if (GameEvent.FromID(eventID) == null && valid) {
			InvocationHelper.If(log, () => UnityEngine.Debug.LogError($"Invalid eventID: {eventID} for EventTimerBufferedTrigger in {stat.GetStatDefinition()} @ {provider}"));
			valid = false; 
		}
		return valid;
	}
}
