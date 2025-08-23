using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public interface IStatEntry {
	void ApplyToSerialized(AbstractSerializableStat serializableStat, IStatProvider source);

	void RevokeToSerialized(AbstractSerializableStat serializableStat, IStatProvider source);


	internal class UnsupportedSerializableStatTypeException : NullReferenceException {
		public UnsupportedSerializableStatTypeException(object value, Type expectedType)
			: base ($"Unsupported stat value type {value.GetType()} for entry of type {expectedType}") {}
	}
}