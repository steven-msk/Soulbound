using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

public interface IStatProcessor<TValue> where TValue : struct, IComparable<TValue> {
	public TValue ProcessFinalValue(TValue baseValue, IEnumerable<TValue> modifiers);
}
