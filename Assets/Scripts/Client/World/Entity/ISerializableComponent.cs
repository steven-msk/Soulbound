using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.EntitySystem {
	public interface ISerializableComponent {
		void Save(SerializedEntityPropertyList list);
		void Read(SerializedEntityPropertyList list);
	}
}
