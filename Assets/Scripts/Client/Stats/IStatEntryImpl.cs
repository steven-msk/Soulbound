using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IStatEntryImpl {
	void Add(AbstractSerializableStat serializableStat, IStatProvider source);

	void AddRange(params (AbstractSerializableStat stat, IStatProvider source)[] modifiers);

	void Remove(AbstractSerializableStat serializableStat, IStatProvider source);

	void RemoveRange(params (AbstractSerializableStat stat, IStatProvider source)[] modifiers);

	void SetModifiers(List<(AbstractSerializableStat stat, IStatProvider source)> modifiers);

	object GetBoxedValue();

	List<(AbstractSerializableStat, IStatProvider)> GetBoxedModifiers();

	internal class UnsupportedSerializableStatTypeException : NullReferenceException {
		public UnsupportedSerializableStatTypeException(object value, Type expectedType)
			: base ($"Unsupported stat value type {value.GetType()} for entry of type {expectedType}") {
		}
	}
}