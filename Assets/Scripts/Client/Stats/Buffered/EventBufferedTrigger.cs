using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

[Obsolete]
public class EventBufferedTrigger : IBufferedTrigger {
	[JsonProperty] private readonly string eventID;
	[JsonProperty] private readonly BufferedTriggerCondition condition;

	public EventBufferedTrigger(string eventID, BufferedTriggerCondition condition) {
		this.eventID = eventID;
		this.condition = condition;
	}

	[JsonIgnore] public Func<bool> InvocationValidator => condition.ToValidator();

	public void Enable(IBufferedStatImpl stat, IStatProvider provider, BufferedTriggerState state) {
		InvocationHelper.If(ValidateExecution(stat, provider, false), () => { 
			EventBus<GameEvent>.Subscribe(GameEvent.FromID(eventID), state.GetInvokeAction(this, stat, provider));
		});
	}

	public void Disable(IBufferedStatImpl stat, IStatProvider provider, BufferedTriggerState state) {
		InvocationHelper.If(ValidateExecution(stat, provider, false), () => {
			EventBus<GameEvent>.Unsubscribe(GameEvent.FromID(eventID), state.GetInvokeAction(this, stat, provider));
		});
	}

	public void Invoke(IBufferedStatImpl stat, Action action) => InvocationHelper.If(InvocationValidator.Invoke(), action);

	public bool ValidateExecution(IBufferedStatImpl stat, IStatProvider provider, bool log) {
		bool valid = true;
		if (string.IsNullOrEmpty(eventID)) {
			InvocationHelper.If(log, () => UnityEngine.Debug.LogError($"Null or empty eventID for EventBufferedTrigger in {stat.GetStatDefinition()} @ {provider}"));
			valid = false;
		}
		if (GameEvent.FromID(eventID) == null && valid){
			InvocationHelper.If(log, () => UnityEngine.Debug.LogError($"Invalid eventID: {eventID} for EventBufferedTrigger in {stat.GetStatDefinition()} @ {provider}"));
			valid = false;
		}
		return valid;
	}
}
