using System;

namespace SoulboundEngine.Common {
	[AttributeUsage(AttributeTargets.Method
		| AttributeTargets.Constructor
		| AttributeTargets.Class
		| AttributeTargets.Interface
		| AttributeTargets.Struct
		| AttributeTargets.Enum
		| AttributeTargets.Event
		| AttributeTargets.Field
		| AttributeTargets.Property, AllowMultiple = false)]
	/// <summary>
	/// Used to indicate something is of a prototypical nature, and is yet to be implemented properly.
	/// Implementations decorated with this attribute are not (yet) subjects to test cases.
	/// </summary>
	public class PROTOTYPICALAttribute : Attribute {
	}
}
