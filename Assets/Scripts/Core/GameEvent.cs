using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

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

	public static GameEvent FromID(string ID) => gameEventsByName[ID];
}