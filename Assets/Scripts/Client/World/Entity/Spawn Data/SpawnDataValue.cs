using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class SpawnDataValue {
	public abstract object GetValue();
}

public class SpawnDataValue<TValue> : SpawnDataValue {
	public TValue value { get; }

	public SpawnDataValue(TValue value) => this.value = value;

	public override object GetValue() => value;
}
