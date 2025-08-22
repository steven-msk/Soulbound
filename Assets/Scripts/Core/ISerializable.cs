using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface ISerializable<T> {
	public T Serialize();

	public void Deserialize(T serialized);
}
