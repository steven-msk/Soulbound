using System;
using UnityEngine;

[Serializable]
public class BufferedStat : SerializableStat {
	[SerializeField] private string applyHook;
	[SerializeField] private string revokeHook;
	[SerializeField] private string applyBufferCondition;
	[SerializeField] private string revokeBufferCondition;
	private bool applied = false;

	// REFACTOR: the whole logic of BufferedStat

	public BufferedStat(SerializedStatReference serializedReference, StatValueType valueType, StatApplicationType appliance, object value, bool applyAsBonus,
						string applyHook, string revokeHook)
			: base(serializedReference, valueType, appliance, value, applyAsBonus) {
		this.applyHook = applyHook;
		this.revokeHook = revokeHook;
	}

	public void SubscribeBuffers(PlayerStats playerStats, IStatProvider source) {
		if (string.IsNullOrEmpty(revokeHook)) {
			Debug.LogWarning($"Missing revokeHook for buffered stat: {serializedReference.ToStatType().BaseName} @ {source}");
			return;
		}
		EventBus<GameEvent>.Subscribe(GameEvent.FromID(revokeHook), () => InvokeBufferCheck(playerStats, source, revokeBufferCondition, false));

		if (string.IsNullOrEmpty(applyHook)) {
			Debug.LogWarning($"Missing applyHook for buffered stat {serializedReference.ToStatType().BaseName} @ {source}");
			return;
		}
		EventBus<GameEvent>.Subscribe(GameEvent.FromID(applyHook), () => InvokeBufferCheck(playerStats, source, applyBufferCondition, true));
	}

	public void UnsubscribeBuffers(PlayerStats playerStats, IStatProvider source) {
		if (string.IsNullOrEmpty(revokeHook)) {
			Debug.LogWarning($"Missing revokeHook for buffered stat {serializedReference.ToStatType().BaseName} @ {source}");
			return;
		}
		EventBus<GameEvent>.Unsubscribe(GameEvent.FromID(revokeHook), () => InvokeBufferCheck(playerStats, source, revokeBufferCondition, false));

		if (string.IsNullOrEmpty(applyHook)) {
			Debug.LogWarning($"Missing applyHook for buffered stat {serializedReference.ToStatType().BaseName} @ {source}");
			return;
		}
		EventBus<GameEvent>.Unsubscribe(GameEvent.FromID(applyHook), () => InvokeBufferCheck(playerStats, source, applyBufferCondition, true));

	}

	private void InvokeBufferCheck(PlayerStats playerStats, IStatProvider source, string bufferCondition, bool apply) {
		if (string.IsNullOrEmpty(bufferCondition)) {
			bufferCondition = "AlwaysTrue";
		}
		if (BufferedConditionRegistry.GetCondition(bufferCondition).Invoke()) {
			InvocationHelper.IfElse(apply, () => {
				if (applied) {
					Debug.LogWarning($"Buffered stat already applied: {serializedReference.ToStatType().BaseName} @ {source}");
					return;
				}
				playerStats.Apply(this, source);
				applied = true;
			}, () => { 
				if (applied) {
					playerStats.Revoke(this, source);
					applied = false;
				} else {
					Debug.LogWarning($"Tried to revoke buffered stat but it has not been applied yet: {serializedReference.ToStatType().BaseName} @ {source}");
				}
			});
		}
	}
}