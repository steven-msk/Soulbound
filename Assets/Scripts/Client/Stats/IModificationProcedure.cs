using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace SoulboundBackend.Client.Stats {
	public interface IModificationProcedure<TValue> where TValue : struct, IComparable<TValue> {
		TValue Apply(TValue currentValue, TValue modifierValue, StatEntry<TValue> entry);
	}
}
