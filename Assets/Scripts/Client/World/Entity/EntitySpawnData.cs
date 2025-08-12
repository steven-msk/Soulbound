using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public record EntitySpawnData {
	public Vector2 position;

	public EntitySpawnData(Vector2 position) {
		this.position = position;
	}
}

public class MismatchedEntitySpawnDataTypeException : Exception {
	public Type expectedData { get; private set; }
	public Type passedType { get; private set; }
	public MismatchedEntitySpawnDataTypeException(Type expectedType, Type passedType) 
		: base($"Mismatched entity spawn data: type of {expectedType} isnt the same as {passedType}") {
		this.expectedData = expectedType;
		this.passedType = passedType;
	}
}
