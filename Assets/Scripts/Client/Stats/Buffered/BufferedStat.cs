using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class BufferedStat<TValue> : SerializableStat<TValue>, IBufferedStatImpl where TValue : struct, IComparable<TValue> {
	public IBufferedTrigger applyBufferedTrigger;
	public IBufferedTrigger revokeBufferedTrigger;

	public BufferedStat(StatDefinition<TValue> statType, TValue value, StatApplicationType appliance, bool applyAsBonus,
						IBufferedTrigger applyBufferedTrigger, IBufferedTrigger revokeBufferedTrigger)
			: base(statType, value, appliance, applyAsBonus) {
		this.applyBufferedTrigger = applyBufferedTrigger;
		this.revokeBufferedTrigger = revokeBufferedTrigger;
	}

	public void EnableBuffers(IStatProvider source) {
		applyBufferedTrigger.Enable(this, source);
		revokeBufferedTrigger.Enable(this, source);
	}

	public void DisableBuffers(IStatProvider source) {
		applyBufferedTrigger.Disable(this, source);
		revokeBufferedTrigger.Disable(this, source);
	}
}