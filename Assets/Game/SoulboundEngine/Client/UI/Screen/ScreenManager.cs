#nullable enable

namespace SoulboundEngine.Client.UI.Screen {
	using SoulboundEngine.Client.Input;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine.UIElements;

	public sealed class ScreenManager {
		private readonly List<ScreenEntry> stack = new();
		private readonly UXMLScreenRoot screenRoot;
		private readonly Stack<IInputFocusable> focusStack = new();

		public ScreenManager(UXMLScreenRoot screenRoot) {
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

			foreach (ScreenEntry entry in this.stack.Reverse<ScreenEntry>()) {
				if (entry.screen.IsOpaque) {
					buffer.ForEach(e => e.handle.Hide());
					buffer.Clear();
				}

				buffer.Insert(0, entry);
			}

			buffer.ForEach(e => e.handle.Show());
		}

		public VisualElement CreateScreenRoot() {
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

		public bool HasKeyboardFocus() {
			bool hasFocusableKeyboard = this.GetFocus(f => f.HasKeyboardFocus())?.HasKeyboardFocus() ?? false;
			return hasFocusableKeyboard || (this.GetActiveScreen()?.HasKeyboardFocus() ?? false);
		}

		public bool IsPointerOverUI() {
			bool hasUIFocus = this.GetFocus(f => f.IsPointerOverUI())?.IsPointerOverUI() ?? false;
			return hasUIFocus || (this.GetActiveScreen()?.IsPointerOverUI() ?? false);
		}

		private IInputFocusable? GetFocus(Predicate<IInputFocusable> predicate) {
			foreach (IInputFocusable focusable in this.focusStack.Reverse()) {
				if (predicate(focusable)) return focusable;
			}
			return null;
		}

		public void PopInputFocus(IInputFocusable focus) {
			if (this.focusStack.TryPeek(out IInputFocusable? top) && ReferenceEquals(top, focus)) {
				this.focusStack.Pop();
			} else {
				IInputFocusable[] remaining = this.focusStack.Where(f => !ReferenceEquals(f, focus)).Reverse().ToArray();
				this.focusStack.Clear();
				foreach (IInputFocusable f in remaining) this.focusStack.Push(f);
			}
		}

		public void PushInputFocus(IInputFocusable focus) {
			this.focusStack.Push(focus);
			Keyboard.ReleaseAll();
		}

		public void IssueRebuild(Screen screen) {
			if (this.GetActiveScreen() != screen) return;
			this.ReplaceScreen(screen);
		}

		public void Flush() {
			foreach (ScreenEntry entry in this.stack) {
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
