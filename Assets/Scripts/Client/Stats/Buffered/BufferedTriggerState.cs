using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Obsolete]
public enum BufferedTriggerState {
	Apply,
	Revoke
}

[Obsolete]
public static class BufferStateAction {
	public static Action GetInvokeAction(this BufferedTriggerState state, IBufferedTrigger trigger, IBufferedStatImpl bufferedStat, IStatProvider provider) {
		PlayerStats playerStats = GameManager.instance.Player.Stats;
		return state switch {
			BufferedTriggerState.Apply => () => trigger.Invoke(bufferedStat, () => playerStats.ApplyProvider(provider)),
			BufferedTriggerState.Revoke => () => trigger.Invoke(bufferedStat, () => playerStats.RevokeProvider(provider)),
			_ => null
		};
	}
}
