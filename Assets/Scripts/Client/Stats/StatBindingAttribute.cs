using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class StatBindingAttribute : Attribute {
	public Type DeclaringType { get; }
	public string FieldName { get; }

	public StatBindingAttribute(Type declaringType, string fieldName) {
		DeclaringType = declaringType;
		FieldName = fieldName;
	}
}
