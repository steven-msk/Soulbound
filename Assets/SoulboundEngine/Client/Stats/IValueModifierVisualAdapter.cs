using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.Stats {
	public interface IValueModifierVisualAdapter<TValue> where TValue : struct, IComparable<TValue> {
		string GetDisplayName(ValueModifier<TValue> modifier);
		string GetDisplayValue(ValueModifier<TValue> modifier);
	}
}
