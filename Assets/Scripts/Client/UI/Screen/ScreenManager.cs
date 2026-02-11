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
	public sealed class ScreenManager : IScreenObjectFactory, IScreenNavigator {
		private readonly Stack<ScreenEntry> stack = new();
		private readonly Transform rootTransform;

		public ScreenManager(Transform rootTransform) {
			this.rootTransform = rootTransform;
		}

		public void PushScreen(Screen screen) {
			if (stack.TryPeek(out ScreenEntry activeEntry)) {
				activeEntry.obj.Hide();
			}

			screen.Init(this);
			IScreenObject obj = screen.BuildObject(this);
			stack.Push(new ScreenEntry(obj));
			obj.Show();
		}

		public bool PopScreen() {
			if (stack.TryPop(out ScreenEntry activeEntry)) {
				activeEntry.obj.Hide();
				activeEntry.obj.Dispose();
			}

			if (stack.TryPeek(out activeEntry)) {
				activeEntry.obj.Show();
			}

			return !stack.IsEmpty();
		}

		public void ReplaceScreen(Screen screen) {
			PopScreen();
			PushScreen(screen);
		}

		public Screen? GetActiveScreen() {
			return stack.TryPeek(out ScreenEntry activeEntry)
				? activeEntry.screen
				: null;
		}

		private IScreenObject? GetActiveObject() {
			return stack.TryPeek(out ScreenEntry activeEntry)
				? activeEntry.obj
				: null;
		}

		public void Flush() {
			while (!stack.IsEmpty()) {
				var screenObject = stack.Pop();
				screenObject.obj.Hide();
				screenObject.obj.Dispose();
			}
		}

		GameObject IScreenObjectFactory.CreateGameObject() {
			GameObject obj = new("Screen Object", typeof(RectTransform));
			obj.transform.SetParent(rootTransform, false);

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
		public Screen screen => obj.GetInstance();
	}
}
