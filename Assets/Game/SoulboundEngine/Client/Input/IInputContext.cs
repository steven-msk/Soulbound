using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.Input {
	public interface IInputContext {
		virtual int priority { get => 0; }
		bool HandleInput(in InputEvent inputEvent);
	}
}
