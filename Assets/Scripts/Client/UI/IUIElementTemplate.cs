using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI {
	public interface IUIElementTemplate<THandle> where THandle : IUIElementHandle {
		GameObject Instantiate();
		virtual void Apply(THandle handle) {
		}
	}
}
