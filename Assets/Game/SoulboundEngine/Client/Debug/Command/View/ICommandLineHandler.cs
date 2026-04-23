using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

namespace SoulboundEngine.Client.Debug.Commands.View {
	public interface ICommandLineHandler {
		void InsertCompletion();
		void ShowCompletions(string value);

		void HandleKey(Key key);
	}
}
