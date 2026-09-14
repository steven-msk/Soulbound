namespace SoulboundEngine.UnityClient.Editor.Tools.WorldGen {
	using SoulboundEngine.UnityClient.Debug.Logging;
	using UnityEditor;
	using UnityEditor.SceneManagement;

	public class EditorWorldGenTool {
		[MenuItem("Soulbound/World gen tools/Switch to WorldGenScene")]
		static void SwitchToTool() {
			EditorSceneManager.OpenScene("Assets/Game/Content/Editor/Tools/WorldGenScene.unity");
			Logger.SetWrapper(new UnityClientLoggerWrapper(UnityEngine.Debug.unityLogger));
			new EditorWorldGenContext().Init();
		}
	}
}
