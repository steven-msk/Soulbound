using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IStatEntryImpl {
	void Add(AbstractSerializableStat serializableStat, IStatProvider provider);

	void AddRange(params (AbstractSerializableStat stat, IStatProvider provider)[] modifiers);

	void Remove(AbstractSerializableStat serializableStat, IStatProvider provider);

	void RemoveRange(params (AbstractSerializableStat stat, IStatProvider provider)[] modifiers);

	void SetModifiers(List<(AbstractSerializableStat stat, IStatProvider provider)> modifiers);

	object GetBoxedValue();

	List<(AbstractSerializableStat, IStatProvider)> GetBoxedModifiers();

	internal class UnsupportedSerializableStatTypeException : NullReferenceException {
		public UnsupportedSerializableStatTypeException(object value, Type expectedType)
			: base ($"Unsupported stat value type {value.GetType()} for entry of type {expectedType}") {
		}
	}
}