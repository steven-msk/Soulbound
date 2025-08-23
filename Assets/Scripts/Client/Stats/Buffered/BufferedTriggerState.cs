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
	public static Action GetInvokeAction(this BufferedTriggerState state, IBufferedTrigger trigger, IBufferedStatImpl bufferedStat, IStatProvider source) {
		PlayerStats playerStats = GameManager.instance.Player.Stats;
		return state switch {
			BufferedTriggerState.Apply => () => trigger.Invoke(bufferedStat, () => playerStats.Apply(bufferedStat.Cast(), source)),
			BufferedTriggerState.Revoke => () => trigger.Invoke(bufferedStat, () => playerStats.Revoke(bufferedStat.Cast(), source)),
			_ => null
		};
	}
}
