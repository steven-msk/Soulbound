using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IEntity {
	Guid id { get; set; }
	Vector2 position { get; set; }

	void Update(float deltaTime);

	void OnChunkLoaded();
	void OnChunkUnloaded();
}
