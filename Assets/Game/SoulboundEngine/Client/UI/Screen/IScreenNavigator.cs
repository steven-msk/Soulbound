namespace SoulboundEngine.Client.UI.Screens {
	public interface IScreenNavigator {
		void PushScreen(Screen screen);
		void ReplaceScreen(Screen screen);
		bool PopScreen();
		void IssueRebuild(Screen screen);
	}
}
