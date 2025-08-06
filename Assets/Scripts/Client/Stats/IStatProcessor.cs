using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

public interface IStatProcessor<TValue> {
	public TValue ProcessFinalValue(TValue baseValue, IEnumerable<TValue> modifiers);
}
