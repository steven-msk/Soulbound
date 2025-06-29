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

	public void Enable(BufferedStat stat, IStatProvider source) {
		EventBus<GameEvent>.Subscribe(GameEvent.FromID(eventID), State.GetInvokeAction(this, stat, source));
	}

	public void Disable(BufferedStat stat, IStatProvider source) {
		EventBus<GameEvent>.Unsubscribe(GameEvent.FromID(eventID), State.GetInvokeAction(this, stat, source));
	}

	public void Invoke(BufferedStat stat, Action action) => InvocationHelper.If(InvocationValidator.Invoke(), action);
}
