namespace SoulboundEngine.UnityClient.Editor.Tools.WorldGen {
	using SoulboundEngine.Registry;
	using SoulboundEngine.UnityClient.Assets;
	using SoulboundEngine.UnityClient.Debug.Logging;
	using SoulboundEngine.World.Gen.Noise;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEditor;
	using UnityEditor.SceneManagement;
	using UnityEngine;
	using UnityEngine.UIElements;

	public sealed class NoiseTunerWindow : EditorWindow {
		private EditorWorldGenContext context;
		private readonly Dictionary<RegistryKey<NormalNoise.Parameters>, NormalNoise.Parameters> overrides = new();

		[MenuItem("Soulbound/World gen tools/Open Noise Tuner")]
		public static void Open() {
			NoiseTunerWindow window = GetWindow<NoiseTunerWindow>();
			window.titleContent = new GUIContent("Noise Tuner");
		}

		public void CreateGUI() {
			SoulboundEngine.Logger.SetWrapper(new UnityClientLoggerWrapper(UnityEngine.Debug.unityLogger));
			EditorSceneManager.OpenScene("Assets/Game/Content/Editor/Tools/WorldGenScene.unity");
			AssetManager.LoadAllWithPreloadLabel();
			this.context = new EditorWorldGenContext();
			this.context.Init();

			ScrollView scroll = new(ScrollViewMode.Vertical);
			scroll.style.flexGrow = 1;
			this.rootVisualElement.Add(scroll);

			foreach ((RegistryKey<NormalNoise.Parameters> key, NormalNoise.Parameters noiseParams) in this.context.Noises.GetEntrySet()) {
				scroll.Add(this.BuildNoiseEditor(key, noiseParams));
			}

			Button regenerate = new(() => EditorWorldGenTool.Regenerate(this.overrides, this.context)) { text = "Regenerate" };
			regenerate.style.height = 32;
			this.rootVisualElement.Add(regenerate);
		}

		private VisualElement BuildNoiseEditor(RegistryKey<NormalNoise.Parameters> key, NormalNoise.Parameters parameters) {
			Foldout foldout = new() { text = key.value.ToString(), value = false };

			IntegerField firstOctaveField = new("First Octave") { value = parameters.firstOctave };
			foldout.Add(firstOctaveField);

			VisualElement multipliersList = new();
			foldout.Add(multipliersList);
			List<FloatField> multiplierFields = new();
			foreach (double multiplier in parameters.octaveMultipliers) {
				multiplierFields.Add(this.AddField(multipliersList, (float)multiplier));
			}
			Button addOctave = new(() => {
				FloatField field = new() { value = 1f };
				field.RegisterValueChangedCallback(_ => Commit());
				multiplierFields.Add(field);
				multipliersList.Add(field); 
			}) { text = "+ Octave" };
			foldout.Add(addOctave);

			void Commit(ChangeEvent<int> _ = default) {
				double[] multipliers = multiplierFields.Select(f => (double)f.value).ToArray();
				this.overrides[key] = NormalNoise.Parameters.Of(firstOctaveField.value, multipliers);
			}
			firstOctaveField.RegisterValueChangedCallback(Commit);
			multiplierFields.ForEach(f => f.RegisterValueChangedCallback(_ => Commit()));

			Button removeOctave = new(() => {
				FloatField field = multiplierFields.Last();
				multiplierFields.RemoveAt(multiplierFields.Count - 1);
				multipliersList.Remove(field);
			}) { text = "- Octave" };
			foldout.Add(removeOctave);

			return foldout;
		}

		private FloatField AddField(VisualElement container, float initial) {
			FloatField field = new() { value = initial };
			container.Add(field);
			return field;
		}
	}
}
