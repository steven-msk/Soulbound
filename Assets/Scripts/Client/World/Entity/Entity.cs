using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
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

	public abstract void Spawn(EntitySpawnData spawnData);

	public virtual void Despawn() {
		GameManager.instance.Level.EntityManager.RemoveEntity(this);
	}

	public void ValidateSpawnData<TData>(EntitySpawnData spawnData, Action<TData> spawn) where TData : EntitySpawnData {
		spawnData.PatternIfElse<TData>(
			(spawnData) => spawn.Invoke(spawnData),
			() => throw new MismatchedEntitySpawnDataTypeException(typeof(TData), spawnData.GetType()));
	}

	public abstract void OnChunkLoaded();
	public abstract void OnChunkUnloaded();

	public abstract Bounds GetBounds();
}

public static class EnityBounds {
	public static Bounds GetBoundsOrFallback(this Entity entity, Vector2 fallbackSize) {
		Collider2D collider = entity.GetComponent<Collider2D>();
		if (collider != null) {
			return collider.bounds;
		}
		return new Bounds(entity.position, fallbackSize);
	}

	public static Bounds GetColliderBounds(this Entity entity) {
		return entity.GetComponent<Collider2D>().bounds;
	}
}
