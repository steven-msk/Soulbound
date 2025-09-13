using System.Collections.Generic;

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