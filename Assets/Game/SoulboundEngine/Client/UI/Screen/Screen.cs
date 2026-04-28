using UnityEngine;

namespace SoulboundEngine.Client.UI.Screens {
	public abstract class Screen {
		protected IScreenNavigator screenNavigator;
		protected bool supportsEscapePop = true;

		public void Init(IScreenNavigator navigator) {
			this.screenNavigator = navigator;
		}

		public virtual IScreenObject BuildObject(IScreenObjectFactory objFactory) {
			GameObject gameObject = objFactory.CreateGameObject();
			IScreenObject obj = objFactory.CreateSceneObject(this, gameObject);
			this.OnBuild(obj);
			return obj;
		}

		public bool SupportsEscapePop() => this.supportsEscapePop;

		protected abstract void OnBuild(IScreenObject screenObject);

		public virtual void OnShow(IScreenObject obj) { }

		public virtual void OnHide(IScreenObject obj) { }

		public virtual void OnDispose(IScreenObject obj) { }
	}
}
