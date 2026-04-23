using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SoulboundEngine.Client.Debug.Commands.View {
	public sealed class CommandLineInputField : TMP_InputField {
		public Func<bool> ShouldBlockNavigation;

		public override void OnUpdateSelected(BaseEventData eventData) {
			if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow) || UnityEngine.Input.GetKeyDown(KeyCode.DownArrow)) {
				if (ShouldBlockNavigation?.Invoke() ?? false) {
					eventData.Use();
					return;
				}
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.Escape)) {
				eventData.Use();
				return;
			}

			base.OnUpdateSelected(eventData);
		}
	}
}
