using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace SoulboundEngine.Client.Stats {
	public interface IStatModificationHost {
		void ApplyModifiers(IStatModificationSource source);
		void RemoveModifiers(IStatModificationSource source);
	}
}
