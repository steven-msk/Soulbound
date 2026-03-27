using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundEngine.Client.UI.Screens {
	public interface IScreenRoot {
		void AttachScreenObject(GameObject screenObject);
	}
}
