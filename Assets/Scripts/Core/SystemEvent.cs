using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SystemEvent : IEvent {
	internal static readonly Dictionary<string, SystemEvent> systemEventsByName = new();

	//...

	private string id;
	public string ID => id;

	public SystemEvent(string id) {
		this.id = id;
		systemEventsByName.Add(id, this);
	}
}