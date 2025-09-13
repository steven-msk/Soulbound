using System;

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

	public void EnableBuffers(IStatProvider provider) {
		applyTrigger.Enable(this, provider, BufferedTriggerState.Apply);
		revokeTrigger.Enable(this, provider, BufferedTriggerState.Revoke);
	}

	public void DisableBuffers(IStatProvider provider) {
		applyTrigger.Disable(this, provider, BufferedTriggerState.Apply);
		revokeTrigger.Disable(this, provider, BufferedTriggerState.Revoke);
	}

	public AbstractSerializableStat GetSerializable() => this as AbstractSerializableStat;
}