using System;

namespace SoulboundEngine.Client.UI.Screen {
	public abstract class Screen {
		private ScreenManager _screenManager;
		protected ScreenManager screenManager { get => this._screenManager ?? throw new InvalidOperationException("Screen is not initialized"); }

		public void Init(ScreenManager screenManager, IScreenHandle screenHandle) {
			this._screenManager = screenManager;
			this.OnBuild(screenHandle);
		}

		public virtual bool ReturnWithEscape => true;

		protected abstract void OnBuild(IScreenHandle handle);

		public virtual void OnShow(IScreenHandle handle) { }

		public virtual void OnHide(IScreenHandle handle) { }

		public virtual void OnDispose(IScreenHandle handle) { }
	}
}
