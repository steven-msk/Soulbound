using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Common {
	[AttributeUsage(AttributeTargets.Method
		| AttributeTargets.Constructor
		| AttributeTargets.Class 
		| AttributeTargets.Interface
		| AttributeTargets.Struct 
		| AttributeTargets.Enum
		| AttributeTargets.Event, AllowMultiple = false)]
	/// <summary>
	/// Used to indicate something is of a prototypical nature, and is yet to be implemented properly.
	/// Implementations decorated with this attribute are not (yet) subjects to test cases.
	/// </summary>
	public class PROTOTYPICALAttribute : Attribute {
	}
}
