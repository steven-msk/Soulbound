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

#nullable enable

	public sealed class NoiseTunerWindow : EditorWindow {
		private const string CACHE_PATH = "Assets/Game/Content/Editor/Tools/WorldGen/TunerCache.asset";
		private NoiseTunerCache cache = null!;
		private EditorWorldGenContext context = null!;
		private readonly Dictionary<RegistryKey<NormalNoise.Parameters>, NormalNoise.Parameters> overrides = new();
		private int chunkCount = 1;
		private long seed = 0L;

		[MenuItem("Soulbound/World gen tools/Open Noise Tuner")]
		public static void Open() {
			NoiseTunerWindow window = GetWindow<NoiseTunerWindow>();
			window.titleContent = new GUIContent("Noise Tuner");
		}

		public void CreateGUI() {
			SoulboundEngine.Logger.SetWrapper(new UnityClientLoggerWrapper(UnityEngine.Debug.unityLogger));
			EditorSceneManager.OpenScene("Assets/Game/Content/Editor/Tools/WorldGen/WorldGenScene.unity");
			AssetManager.LoadAllWithPreloadLabel();
			this.context = new EditorWorldGenContext();
			this.context.Init();

			NoiseTunerCache cache = AssetDatabase.LoadAssetAtPath<NoiseTunerCache>(CACHE_PATH);
			if (cache) {
				this.cache = cache;
				this.seed = this.cache.seed;
				this.chunkCount = this.cache.chunkCount;
				this.LoadOverridesFromCache(this.cache);
			} else {
				this.cache = ScriptableObject.CreateInstance<NoiseTunerCache>();
				AssetDatabase.CreateAsset(this.cache, CACHE_PATH);
			}

			ScrollView scroll = new(ScrollViewMode.Vertical);
			scroll.style.flexGrow = 1;
			this.rootVisualElement.Add(scroll);
			this.BuildNoises(scroll);

			IntegerField chunkCount = new("Chunk count") { value = this.chunkCount };
			chunkCount.RegisterValueChangedCallback(v => {
				this.cache.chunkCount = this.chunkCount = v.newValue;
				this.RecreateCacheIfNeeded();
				EditorUtility.SetDirty(this.cache);
				AssetDatabase.SaveAssetIfDirty(this.cache);
			});
			Button regenerate = new(() => EditorWorldGenTool.Regenerate(this.seed, this.chunkCount, this.overrides, this.context)) { text = "Regenerate" };
			regenerate.style.height = 32;
			this.rootVisualElement.Add(regenerate);
			this.rootVisualElement.Add(chunkCount);

			LongField seed = new("Seed") { value = this.seed };
			seed.RegisterValueChangedCallback(v => {
				this.cache.seed = this.seed = v.newValue;
				this.RecreateCacheIfNeeded();
				EditorUtility.SetDirty(this.cache);
				AssetDatabase.SaveAssetIfDirty(this.cache);
			});
			this.rootVisualElement.Add(seed);

			Button resetNoises = new(() => {
				this.overrides.Clear();
				this.RecreateCacheIfNeeded();
				this.cache.overrideEntries.Clear();
				scroll.Clear();
				this.BuildNoises(scroll);
			}) { text = "Reset noises" };
			this.rootVisualElement.Add(resetNoises);
		}

		private void BuildNoises(ScrollView scroll) {
			foreach ((RegistryKey<NormalNoise.Parameters> key, NormalNoise.Parameters noiseParams) in this.context.Noises.GetEntrySet()) {
				scroll.Add(this.BuildNoiseEditor(key, this.overrides.TryGetValue(key, out NormalNoise.Parameters overriden) ? overriden : noiseParams));
			}
		}

		private void LoadOverridesFromCache(NoiseTunerCache cache) {
			this.overrides.Clear();
			foreach (NoiseOverrideEntry entry in cache.overrideEntries) {
				RegistryKey<NormalNoise.Parameters> key = RegistryKey<NormalNoise.Parameters>.Of(RegistryKeys.NOISE, Identifier.Of(entry.registryKey));
				this.overrides[key] = NormalNoise.Parameters.Of(entry.firstOctave, entry.octaveMultipliers);
			}
		}

		private void SaveOverridesToCache(NoiseTunerCache cache) {
			cache.overrideEntries.Clear();
			foreach ((RegistryKey<NormalNoise.Parameters> key, NormalNoise.Parameters parameters) in this.overrides) {
				cache.overrideEntries.Add(new NoiseOverrideEntry {
					registryKey = key.value.ToString(),
					firstOctave = parameters.firstOctave,
					octaveMultipliers = parameters.octaveMultipliers
				});
			}
		}

		private void RecreateCacheIfNeeded() {
			if (!this.cache || !AssetDatabase.AssetPathExists(CACHE_PATH)) {
				this.cache = ScriptableObject.CreateInstance<NoiseTunerCache>();
				AssetDatabase.CreateAsset(this.cache, CACHE_PATH);
			}
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

			void Commit(ChangeEvent<int> _ = default!) {
				double[] multipliers = multiplierFields.Select(f => (double)f.value).ToArray();
				this.overrides[key] = this.overrides[key] = NormalNoise.Parameters.Of(firstOctaveField.value, multipliers);
				this.RecreateCacheIfNeeded();
				this.SaveOverridesToCache(this.cache);
				EditorUtility.SetDirty(this.cache);
				AssetDatabase.SaveAssetIfDirty(this.cache);
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
			field.RegisterValueChangedCallback(_ => { });
			container.Add(field);
			return field;
		}
	}
}
