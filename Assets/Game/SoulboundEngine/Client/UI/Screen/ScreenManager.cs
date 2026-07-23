using SoulboundEngine.Client.Debug.Logging;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

#nullable enable

namespace SoulboundEngine.Client.UI.Screen {
	public sealed class ScreenManager : IScreenNavigator {
		private readonly List<ScreenEntry> stack = new();
		private readonly UIToolkitScreenRoot screenRoot;

		public ScreenManager(UIToolkitScreenRoot screenRoot) {
			this.screenRoot = screenRoot;
		}

		public IScreenHandle PushScreen(Screen screen) {
			VisualElement root = this.CreateScreenRoot();
			
			IScreenHandle handle = new UXMLScreenHandle(screen, root);
			screen.Init(this, handle);

			this.screenRoot.Attach(root);

			this.stack.Insert(0, new ScreenEntry(handle));
			this.Render();

			return handle;
		}

		private void Render() {
			List<ScreenEntry> buffer = new();

			foreach (var entry in this.stack.Reverse<ScreenEntry>()) {
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

		public bool PopTopScreen() {
			ScreenEntry? topEntry = this.GetTopEntry();
			if (topEntry == null) return false;

			if (this.stack.Remove(topEntry)) {
				this.HideAndDispose(topEntry);
			}

			this.Render();

			return this.stack.Any();
		}

		public void PopScreen(IScreenHandle handle) {
			ScreenEntry entry = this.stack.FirstOrDefault(e => e.handle == handle);
			if (entry == null) {
				Logger.LogError("Could not find screen handle");
				return;
			}
			this.stack.Remove(entry);
			this.HideAndDispose(entry);
			this.Render();
		}

		public void ReplaceScreen(Screen screen) {
			this.PopTopScreen();
			this.PushScreen(screen);
		}

		public Screen? GetActiveScreen() => this.GetTopEntry()?.screen;

		private ScreenEntry? GetTopEntry() => this.stack.First();

		public void IssueRebuild(Screen screen) {
			if (this.GetActiveScreen() != screen) return;
			this.ReplaceScreen(screen);
		}

		public void Flush() {
			foreach (var entry in this.stack) {
				this.HideAndDispose(entry);
			}
			this.stack.Clear();
		}

		private void HideAndDispose(ScreenEntry entry) {
			entry.handle.Hide();
			entry.handle.Dispose();
		}
	}

	sealed record ScreenEntry(IScreenHandle handle) {
		public Screen screen => this.handle.GetScreen();
	}
}
