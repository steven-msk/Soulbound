using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EventBufferedTrigger : IBufferedTrigger {
	[SerializeField] private string eventID;
	Func<bool> IBufferedTrigger.InvocationValidator => throw new NotImplementedException();

	void IBufferedTrigger.Disable(BufferedStat stat) {
		throw new NotImplementedException();
	}

	void IBufferedTrigger.Enable(BufferedStat stat) {
		throw new NotImplementedException();
	}

	void IBufferedTrigger.Invoke(BufferedStat stat, Action action) {
		throw new NotImplementedException();
	}
}
