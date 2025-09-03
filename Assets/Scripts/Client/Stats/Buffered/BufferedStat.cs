using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

[Obsolete]
public class BufferedStat<TValue> : SerializableStat<TValue>, IBufferedStatImpl where TValue : struct, IComparable<TValue> {
	public IBufferedTrigger applyTrigger;
	public IBufferedTrigger revokeTrigger;

	public BufferedStat(StatDefinition<TValue> statType, TValue value, StatApplicationType appliance, bool applyAsBonus,
						IBufferedTrigger applyBufferedTrigger, IBufferedTrigger revokeBufferedTrigger)
			: base(statType, value, appliance, applyAsBonus) {
		this.applyTrigger = applyBufferedTrigger;
		this.revokeTrigger = revokeBufferedTrigger;
	}

	public void EnableBuffers(IStatProvider source) {
		applyTrigger.Enable(this, source, BufferedTriggerState.Apply);
		revokeTrigger.Enable(this, source, BufferedTriggerState.Revoke);
	}

	public void DisableBuffers(IStatProvider source) {
		applyTrigger.Disable(this, source, BufferedTriggerState.Apply);
		revokeTrigger.Disable(this, source, BufferedTriggerState.Revoke);
	}

	public AbstractSerializableStat GetSerializable() => this as AbstractSerializableStat;
}