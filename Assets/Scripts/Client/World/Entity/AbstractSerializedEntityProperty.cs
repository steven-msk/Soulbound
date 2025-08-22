using Codice.CM.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Profiling;

public abstract class AbstractSerializedEntityProperty {
	public abstract object GetValueAsObject();
	public abstract void SetValueFromObject(object value);
	public TValue GetValue<TValue>() => (TValue)GetValueAsObject();
	public abstract SpawnDataValue ToSpawnDataValue();
	public abstract string GetKey();
}
