using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class AbstractStatEntry<TValue> {
	public TValue BaseValue { get; protected set; }

	protected AbstractStatEntry(TValue baseValue) {
		this.BaseValue = baseValue;
	}
}
