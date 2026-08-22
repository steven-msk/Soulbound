namespace SoulboundEngine.UnityClient.UI.Screen {
	public interface IScreenNavigator {
		IScreenHandle PushScreen(Screen screen);
		void ReplaceScreen(Screen screen);
		bool PopTopScreen();
		void IssueRebuild(Screen screen);
		void PopScreen(IScreenHandle handle);
	}
}
