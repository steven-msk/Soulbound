using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum BufferedTriggerState {
	Apply,
	Revoke
}

public static class BufferStateAction {
	public static Action GetInvokeAction(this BufferedTriggerState state, IBufferedTrigger trigger, BufferedStat bufferedStat, IStatProvider source) {
		PlayerStats playerStats = GameManager.GetPlayerInstance().Stats;
		return state switch {
			BufferedTriggerState.Apply => () => trigger.Invoke(bufferedStat, () => playerStats.Apply(bufferedStat, source)),
			BufferedTriggerState.Revoke => () => trigger.Invoke(bufferedStat, () => playerStats.Revoke(bufferedStat, source)),
			_ => null
		};
	}
}
