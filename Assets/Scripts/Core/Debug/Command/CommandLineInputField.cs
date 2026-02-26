using SoulboundBackend.Core.Debug.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Logger = SoulboundBackend.Core.Debug.Logging.Logger;

namespace SoulboundBackend.Core.Debug.Commands {
	public sealed class CommandLineInputField : TMP_InputField {
		public Func<bool> ShouldBlockNavigation;

		public override void OnUpdateSelected(BaseEventData eventData) {
			if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)) {
				if (ShouldBlockNavigation?.Invoke() ?? false) {
					eventData.Use();
					return;
				}
			}
			if (Input.GetKeyDown(KeyCode.Escape)) {
				eventData.Use();
				return;
			}

			base.OnUpdateSelected(eventData);
		}
	}
}
