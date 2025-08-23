using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EventBufferedTrigger : IBufferedTrigger {
	[SerializeField] private string eventID;
	[SerializeField] private BufferedTriggerCondition condition;

	public BufferedTriggerState State { get; set; }

	public Func<bool> InvocationValidator => condition.ToValidator();

	public void Enable(IBufferedStatImpl stat, IStatProvider source) {
		InvocationHelper.If(ValidateExecution(stat, source, false), () => { 
			EventBus<GameEvent>.Subscribe(GameEvent.FromID(eventID), State.GetInvokeAction(this, stat, source));
		});
	}

	public void Disable(IBufferedStatImpl stat, IStatProvider source) {
		InvocationHelper.If(ValidateExecution(stat, source, false), () => {
			EventBus<GameEvent>.Unsubscribe(GameEvent.FromID(eventID), State.GetInvokeAction(this, stat, source));
		});
	}

	public void Invoke(IBufferedStatImpl stat, Action action) => InvocationHelper.If(InvocationValidator.Invoke(), action);

	public bool ValidateExecution(IBufferedStatImpl stat, IStatProvider source, bool log) {
		bool valid = true;
		if (string.IsNullOrEmpty(eventID)) {
			InvocationHelper.If(log, () => UnityEngine.Debug.LogError($"Null or empty eventID for EventBufferedTrigger in {stat.GetStatDefinition()} @ {source}"));
			valid = false;
		}
		if (GameEvent.FromID(eventID) == null && valid){
			InvocationHelper.If(log, () => UnityEngine.Debug.LogError($"Invalid eventID: {eventID} for EventBufferedTrigger in {stat.GetStatDefinition()} @ {source}"));
			valid = false;
		}
		return valid;
	}
}
