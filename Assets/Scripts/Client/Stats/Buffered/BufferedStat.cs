using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

[Serializable]
public class BufferedStat : SerializableStat {
	[SerializeReference] private IBufferedTrigger applyBufferedTrigger;
	[SerializeReference] private IBufferedTrigger revokeBufferedTrigger;
	private bool applied = false;

	// TODO: add BufferedStat documentation

	public BufferedStat(SerializedStatReference serializedReference, StatValueType valueType, StatApplicationType appliance, object value, bool applyAsBonus,
						IBufferedTrigger applyBufferedTrigger, IBufferedTrigger revokeBufferedTrigger)
			: base(serializedReference, valueType, appliance, value, applyAsBonus) {
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