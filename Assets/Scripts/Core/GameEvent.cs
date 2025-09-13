using JetBrains.Annotations;
using System.Collections.Generic;

public class GameEvent : IEvent {
	internal static readonly Dictionary<string, GameEvent> gameEventsByName = new();
	
	public static readonly GameEvent PlayerAttackStart = new("PlayerAttackStart");
	public static readonly GameEvent PlayerAttackEnd = new("PlayerAttackEnd");
	//...

	private string id;
	public string ID => id;

	public GameEvent(string id) {
		this.id = id;
		gameEventsByName[id] = this;
	}

	[CanBeNull] public static GameEvent FromID(string ID) {
		if (gameEventsByName.ContainsKey(ID)) {
			return gameEventsByName[ID];
		}
		return null;
	}
}