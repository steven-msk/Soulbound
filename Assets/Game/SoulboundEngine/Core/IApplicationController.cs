using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Core {
	public interface IApplicationController {
		void EnterWorld(string world);
		public void QuitActiveWorld();

		// application lifetime
		void Launch();
		void OnApplicationQuit();
	}
}
