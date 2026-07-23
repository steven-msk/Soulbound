using SoulboundEngine.Common;
using System;

namespace SoulboundEngine.Client.UI.Screen {
	public abstract class Screen {
		private ScreenManager screenManager;
		protected ScreenManager ScreenManager { get => this.screenManager ?? throw new InvalidOperationException("Screen is not initialized"); }

		public void Init(ScreenManager screenManager, IScreenHandle screenHandle) {
			this.screenManager = screenManager;
			this.OnBuild(screenHandle);
		}

		public virtual bool CloseOnEsc => true;
		public virtual bool IsOpaque => true;

		protected abstract void OnBuild(IScreenHandle handle);

		public virtual void OnShow(IScreenHandle handle) { }

		public virtual void OnHide(IScreenHandle handle) { }

		public virtual void OnDispose(IScreenHandle handle) { }

		[PROTOTYPICAL] public abstract void SetTooltip(string text);
		[PROTOTYPICAL] public abstract void ClearTooltip();
	}
}
