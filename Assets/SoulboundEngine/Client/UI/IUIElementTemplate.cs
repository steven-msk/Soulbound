using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundEngine.Client.UI {
	public interface IUIElementTemplate<THandle> where THandle : IUIElementHandle {
		GameObject Instantiate();
		virtual void Apply(THandle handle) {
		}

		// a note on an element's layout and visual structure:
		// the current implementation doesnt support any special visual stuff
		// one of the problems is that Unity's layout components do not mix well with visuals and FX
		// because of that, UI visuals will be delayed for production, among other artistic-related features
		// as of the backend for ui visuals, a decorator could handle the internal layout of an element
	}
}
