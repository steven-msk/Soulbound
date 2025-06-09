using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class AbstractStatEntry<TValue> {
	public TValue BaseValue { get; protected set; }
	public StatType<TValue> TypeReference { get; protected set; }

	protected AbstractStatEntry(TValue baseValue, StatType<TValue> typeReference) {
		this.BaseValue = baseValue;
		this.TypeReference = typeReference;
	}
}
