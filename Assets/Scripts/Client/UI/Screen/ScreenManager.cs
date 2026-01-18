using ModestTree;
using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable

namespace SoulboundBackend.Client.UI.Screens {
	public sealed class ScreenManager {
		private Stack<ScreenObject> stack = new();
		private readonly Transform rootTransform;

		public ScreenManager(Transform rootTransform) {
			this.rootTransform = rootTransform;
		}

		public void SetScreen(Screen? screen) {
			if (stack.TryPeek(out var activeScreen)) {
				activeScreen.Hide();
			}

			if (screen != null) {
				ScreenObject screenObject = screen.BuildObject(rootTransform);
				stack.Push(screenObject);
				screenObject.Show();
			} else {
				DisposeStack();
			}
		}

		// will be replaced with a proper screen backtrack method
		// for now this is marked as prototypical
		[PROTOTYPICAL]
		public bool OnEscPressed() {
			if (stack.TryPop(out var activeScreen)) {
				activeScreen.Hide();
				activeScreen.Dispose();
			}

			if (stack.TryPeek(out activeScreen)) {
				activeScreen.Show();
			}

			return !stack.IsEmpty();
		}

		public Screen? GetActiveScreen() {
			return stack.TryPeek(out var activeScreen)
				? activeScreen.GetInstance()
				: null;
		}

		public void DisposeStack() {
			while (!stack.IsEmpty()) {
				var screenObject = stack.Pop();
				screenObject.Hide();
				screenObject.Dispose();
			}
		}
	}
}
