using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable

public abstract class Block {
	public abstract string name { get; }
	public abstract TileBase tileReference { get; }
	public abstract BlockItem? itemReference { get; }
	public abstract BlockState defaultState { get; }

	public virtual BlockState CreateState(Dictionary<string, object> properties) {
		return defaultState;
	}

    public override string ToString() {
		return name;
    }
}