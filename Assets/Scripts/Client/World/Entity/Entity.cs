using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Entity : MonoBehaviour {
	public Guid id { get; set; }
	public Vector2 position => transform.position;
	public int currentChunkX { get; private set; }

	public abstract void EntityUpdate(float deltaTime);

	protected virtual void Start() {
		currentChunkX = ChunkWorldPos.FromWorld(position).chunkX;
	}

	protected virtual void Update() {
		ChunkWorldPos currentPos = ChunkWorldPos.FromWorld(this.position);
		if (currentPos.chunkX != currentChunkX) {
			GameManager.instance.Level.EntityManager.HandleChunkChange(this, currentChunkX, currentPos.chunkX);
			currentChunkX = currentPos.chunkX;
		}
	}

	public abstract void OnChunkLoaded();
	public abstract void OnChunkUnloaded();
}
