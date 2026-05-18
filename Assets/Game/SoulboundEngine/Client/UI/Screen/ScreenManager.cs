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
			VisualElement root = this.CreateScreenRoot();
			
			IScreenHandle handle = new UIToolkitScreenHandle(screen, root);
			screen.Init(this, handle);

			this.screenRoot.Attach(root);

			this.stack.Push(new ScreenEntry(handle));
			this.Render();
		}

		private void Render() {
			List<ScreenEntry> buffer = new();

			foreach (var entry in this.stack.Reverse()) {
				if (entry.screen.IsOpaque) {
					buffer.ForEach(e => e.handle.Hide());
					buffer.Clear();
				}

				buffer.Insert(0, entry);
			}

			buffer.ForEach(e => e.handle.Show());
		}

		private VisualElement CreateScreenRoot() {
			VisualElement root = new() {
				name = "ScreenRoot",
			};
			root.style.flexGrow = 1;
			root.style.position = Position.Absolute;
			root.style.top = root.style.right = root.style.bottom = root.style.left = 0;
			return root;
		}

		public bool PopScreen() {
			if (this.stack.TryPop(out ScreenEntry activeEntry)) {
				activeEntry.handle.Hide();
				activeEntry.handle.Dispose();
			}

			this.Render();

			return this.stack.Any();
		}

		public void ReplaceScreen(Screen screen) {
			this.PopScreen();
			this.PushScreen(screen);
		}

		public void AddOverlay(VisualElement element) {
			if (this.stack.TryPeek(out ScreenEntry activeEntry)) {
				activeEntry.handle.AddOverlay(element);
			}
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
				screenObject.handle.Hide();
				screenObject.handle.Dispose();
			}
		}
	}

	sealed record ScreenEntry(IScreenHandle handle) {
		public Screen screen => this.handle.GetScreen();
	}
}
