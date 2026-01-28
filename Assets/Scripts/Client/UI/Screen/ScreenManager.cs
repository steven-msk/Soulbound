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
	sealed record ScreenEntry(ScreenObject obj) {
		public Screen screen => obj.GetInstance();
	}

	public sealed class ScreenManager {
		private readonly Stack<ScreenEntry> stack = new();
		private readonly Transform rootTransform;

		public ScreenManager(Transform rootTransform) {
			this.rootTransform = rootTransform;
		}

		public void PushScreen(Screen screen) {
			if (stack.TryPeek(out var activeEntry)) {
				activeEntry.obj.Hide();
			}

			ScreenObject obj = screen.BuildObject(rootTransform);
			stack.Push(new ScreenEntry(obj));
			obj.Show();
		}

		public bool PopScreen() {
			if (stack.TryPop(out var activeEntry)) {
				activeEntry.obj.Hide();
				activeEntry.obj.Dispose();
			}

			if (stack.TryPeek(out activeEntry)) {
				activeEntry.obj.Show();
			}

			return !stack.IsEmpty();
		}

		public Screen? GetActiveScreen() {
			return stack.TryPeek(out var activeEntry)
				? activeEntry.screen
				: null;
		}

		public void Flush() {
			while (!stack.IsEmpty()) {
				var screenObject = stack.Pop();
				screenObject.obj.Hide();
				screenObject.obj.Dispose();
			}
		}
	}
}
