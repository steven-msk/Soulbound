using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable

public abstract partial class Block {
	public abstract string name { get; }
	[JsonIgnore] public abstract TileBase tileReference { get; }
	[JsonIgnore] public abstract BlockItem? itemReference { get; }
	[JsonIgnore] public abstract BlockState defaultState { get; }

	public virtual BlockState CreateState(Dictionary<string, object> properties) {
		return defaultState;
	}

    public override string ToString() {
		return name;
    }
}