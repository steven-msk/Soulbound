using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

#nullable enable

namespace SoulboundEngine.Client.UI.Screen {
	public sealed class ScreenManager : IScreenNavigator {
		private readonly Stack<ScreenEntry> stack = new();
		private readonly UIToolkitScreenRoot screenRoot;

		public ScreenManager(UIToolkitScreenRoot screenRoot) {
			this.screenRoot = screenRoot;
		}

		public void PushScreen(Screen screen) {
			if (this.stack.TryPeek(out ScreenEntry activeEntry)) {
				activeEntry.obj.Hide();
			}

			VisualElement root = this.CreateScreenRoot();
			
			IScreenHandle handle = new UIToolkitScreenHandle(screen, root);
			screen.Init(this, handle);

			this.screenRoot.Attach(root);

			this.stack.Push(new ScreenEntry(handle));
			handle.Show();
		}

		private VisualElement CreateScreenRoot() {
			VisualElement root = new() {
				name = "ScreenRoot",
			};
			root.style.flexGrow = 1;
			return root;
		}

		public bool PopScreen() {
			if (this.stack.TryPop(out ScreenEntry activeEntry)) {
				activeEntry.obj.Hide();
				activeEntry.obj.Dispose();
			}

			if (this.stack.TryPeek(out activeEntry)) {
				activeEntry.obj.Show();
			}

			return this.stack.Any();
		}

		public void ReplaceScreen(Screen screen) {
			this.PopScreen();
			this.PushScreen(screen);
		}

		public Screen? GetActiveScreen() {
			return this.stack.TryPeek(out ScreenEntry activeEntry)
				? activeEntry.screen
				: null;
		}

		public void IssueRebuild(Screen screen) {
			if (this.GetActiveScreen() != screen) return;
			this.ReplaceScreen(screen);
		}

		public void Flush() {
			while (this.stack.Count > 0) {
				var screenObject = this.stack.Pop();
				screenObject.obj.Hide();
				screenObject.obj.Dispose();
			}
		}
	}

	sealed record ScreenEntry(IScreenHandle obj) {
		public Screen screen => this.obj.GetScreen();
	}
}
