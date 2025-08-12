using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Entity : MonoBehaviour {
	public Guid id { get; set; }
	public Vector2 position => transform.position;

	public abstract void EntityUpdate(float deltaTime);

	public abstract void OnChunkLoaded();
	public abstract void OnChunkUnloaded();
}
