using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.UI.Screens {
	public sealed class ScreenManager : IScreenObjectFactory, IScreenNavigator {
		private readonly Stack<ScreenEntry> stack = new();
		private readonly IScreenRoot root;

		public ScreenManager(IScreenRoot root) {
			this.root = root;
		}

		public void PushScreen(Screen screen) {
			if (this.stack.TryPeek(out ScreenEntry activeEntry)) {
				activeEntry.obj.Hide();
			}

			screen.Init(this);
			IScreenObject obj = screen.BuildObject(this);
			this.stack.Push(new ScreenEntry(obj));
			obj.Show();
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

		GameObject IScreenObjectFactory.CreateGameObject() {
			GameObject obj = new("Screen Object", typeof(RectTransform));
			this.root.AttachScreenObject(obj);

			// default screen layout
			RectTransform rectTransform = obj.GetComponent<RectTransform>();
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.pivot		= new Vector2(0.5f, 0.5f);
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;
			rectTransform.sizeDelta = Vector2.zero;

			return obj;
		}

		IScreenObject IScreenObjectFactory.CreateSceneObject(Screen screen, GameObject gameObject) {
			ScreenObject screenObject = gameObject.AddComponent<ScreenObject>();
			screenObject.Init(screen);
			return screenObject;
		}
	}

	sealed record ScreenEntry(IScreenObject obj) {
		public Screen screen => this.obj.GetInstance();
	}
}
