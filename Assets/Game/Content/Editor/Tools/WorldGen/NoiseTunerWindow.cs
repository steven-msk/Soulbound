namespace SoulboundEngine.UnityClient.Editor.Tools.WorldGen {
	using Cysharp.Threading.Tasks;
	using SoulboundEngine.Common;
	using SoulboundEngine.Registry;
	using SoulboundEngine.UnityClient.Assets;
	using SoulboundEngine.UnityClient.Debug.Logging;
	using SoulboundEngine.World.Gen.Noise;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEditor;
	using UnityEditor.SceneManagement;
	using UnityEngine;
	using UnityEngine.UIElements;

#nullable enable

	public sealed class NoiseTunerWindow : EditorWindow {
		private const string CACHE_PATH = "Assets/Game/Content/Editor/Tools/WorldGen/TunerCache.asset";
		private const string OCTAVE_LABEL_FORMAT = "2^{}x{} = {}";
		private NoiseTunerCache cache = null!;
		private EditorWorldGenContext context = null!;
		private readonly Dictionary<RegistryKey<NormalNoise.Parameters>, NormalNoise.Parameters> overrides = new();
		private int chunkCount = 1;
		private long seed = 0L;
		private int chunkStartX = 0;
		private int maxConcurrentChunks = 5;
		private int targetState = 3;

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
				this.chunkStartX = this.cache.chunkStartX;
				this.maxConcurrentChunks = cache.maxConcurrentChunks;
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
			Button regenerate = new(
				() => EditorWorldGenTool.Regenerate(this.targetState, this.maxConcurrentChunks, this.seed, this.chunkCount, this.chunkStartX, this.overrides, this.context)
					.Forget(SoulboundEngine.Logger.LogFatal)
			) { text = "Regenerate" };
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

			IntegerField chunkStartX = new("Chunk Start X") { value = this.chunkStartX };
			chunkStartX.RegisterValueChangedCallback(v => {
				this.cache.chunkStartX = this.chunkStartX = v.newValue;
				this.RecreateCacheIfNeeded();
				EditorUtility.SetDirty(this.cache);
				AssetDatabase.SaveAssetIfDirty(this.cache);
			});
			this.rootVisualElement.Add(chunkStartX);

			Button resetNoises = new(() => {
				this.overrides.Clear();
				this.RecreateCacheIfNeeded();
				scroll.Clear();
				this.BuildNoises(scroll);
				this.SaveOverridesToCache(this.cache);
			}) { text = "Reset noises" };
			this.rootVisualElement.Add(resetNoises);

			IntegerField maxConcurrentChunks = new("Max Concurrent Chunks") { value = this.maxConcurrentChunks };
			maxConcurrentChunks.RegisterValueChangedCallback(v => {
				this.cache.maxConcurrentChunks = this.maxConcurrentChunks = v.newValue;
				this.RecreateCacheIfNeeded();
				EditorUtility.SetDirty(this.cache);
				AssetDatabase.SaveAssetIfDirty(this.cache);
			});
			this.rootVisualElement.Add(maxConcurrentChunks);

			IntegerField targetField = new("Target state") { value = this.targetState };
			targetField.RegisterValueChangedCallback(v => {
				this.cache.targetState = this.targetState = v.newValue;
				this.RecreateCacheIfNeeded();
				EditorUtility.SetDirty(this.cache);
				AssetDatabase.SaveAssetIfDirty(this.cache);
			});
			this.rootVisualElement.Add(targetField);
		}

		private void BuildNoises(ScrollView scroll) {
			foreach ((RegistryKey<NormalNoise.Parameters> key, NormalNoise.Parameters noiseParams) in this.context.Noises.GetEntrySet()) {
				scroll.Add(this.BuildNoiseEditor(key, this.overrides.TryGetValue(key, out NormalNoise.Parameters overriden) ? overriden : noiseParams, noiseParams));
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
			EditorUtility.SetDirty(cache);
			AssetDatabase.SaveAssetIfDirty(cache);
		}

		private void RecreateCacheIfNeeded() {
			if (!this.cache || !AssetDatabase.AssetPathExists(CACHE_PATH)) {
				this.cache = ScriptableObject.CreateInstance<NoiseTunerCache>();
				AssetDatabase.CreateAsset(this.cache, CACHE_PATH);
			}
		}

		private VisualElement BuildNoiseEditor(RegistryKey<NormalNoise.Parameters> key, NormalNoise.Parameters parameters, NormalNoise.Parameters baseParameters) {
			Foldout foldout = new() { text = key.value.ToString(), value = false };

			IntegerField firstOctaveField = new("First Octave") { value = parameters.firstOctave };
			foldout.Add(firstOctaveField);

			VisualElement multipliersList = new();
			foldout.Add(multipliersList);
			List<FloatField> multiplierFields = new();
			Dictionary<FloatField, Action<int>> octaveFieldChangeDispatcher = new();
			for (int i = 0; i < parameters.octaveMultipliers.Length; i++) {
				float multiplier = (float)parameters.octaveMultipliers[i];
				FloatField field = this.AddOctaveField(multipliersList, multiplier, i, parameters.firstOctave, out Action<int> onFirstOctaveChanged);
				multiplierFields.Add(field);
				octaveFieldChangeDispatcher.Add(field, onFirstOctaveChanged);
			}

			void Commit(ChangeEvent<int> _ = default!) {
				double[] multipliers = multiplierFields.Select(f => (double)f.value).ToArray();
				this.overrides[key] = this.overrides[key] = NormalNoise.Parameters.Of(firstOctaveField.value, multipliers);
				this.RecreateCacheIfNeeded();
				this.SaveOverridesToCache(this.cache);
			}
			firstOctaveField.RegisterValueChangedCallback(v => {
				int value = v.newValue;
				Commit();
				foreach ((_, Action<int> action) in octaveFieldChangeDispatcher) {
					action(value);
				}
				firstOctaveField.label = value == baseParameters.firstOctave ? "First Octave" : $"First Octave ({baseParameters.firstOctave})";
			});
			multiplierFields.ForEach(f => f.RegisterValueChangedCallback(_ => Commit()));

			Button addOctave = new(() => {
				FloatField field = this.AddOctaveField(multipliersList, 1f, multiplierFields.Count, parameters.firstOctave, out Action<int> onFirstOctaveChanged);
				octaveFieldChangeDispatcher.Add(field, onFirstOctaveChanged);
				field.RegisterValueChangedCallback(_ => Commit());
				multiplierFields.Add(field);
				Commit();
			}) { text = "+ Octave" };
			foldout.Add(addOctave);

			Button removeOctave = new(() => {
				FloatField field = multiplierFields.Last();
				multiplierFields.RemoveAt(multiplierFields.Count - 1);
				multipliersList.Remove(field);
				octaveFieldChangeDispatcher.Remove(field);
				Commit();
			}) { text = "- Octave" };
			foldout.Add(removeOctave);

			Button reset = new(() => {
				multiplierFields.Clear();
				multipliersList.Clear();
				octaveFieldChangeDispatcher.Clear();
				for (int i = 0; i < baseParameters.octaveMultipliers.Length; i++) {
					float multiplier = (float)baseParameters.octaveMultipliers[i];
					FloatField field = this.AddOctaveField(multipliersList, multiplier, i, baseParameters.firstOctave, out Action<int> onFirstOctaveChanged);
					octaveFieldChangeDispatcher.Add(field, onFirstOctaveChanged);
					field.RegisterValueChangedCallback(_ => Commit());
					multiplierFields.Add(field);
				}
				firstOctaveField.value = baseParameters.firstOctave;
				this.overrides.Remove(key);
				this.SaveOverridesToCache(this.cache);
			}) { text = "Reset" };
			foldout.Add(reset);

			return foldout;
		}

		private FloatField AddOctaveField(VisualElement container, float initial, int index, int firstOctave, out Action<int> firstOctaveChanged) {
			FloatField field = new() { value = initial };
			this.UpdateOctaveLabel(field, initial, initial, index, firstOctave);
			field.RegisterValueChangedCallback(v => this.UpdateOctaveLabel(field, initial, v.newValue, index, firstOctave));
			container.Add(field);
			firstOctaveChanged = value => this.UpdateOctaveLabel(field, initial, field.value, index, value);
			return field;
		}

		private void UpdateOctaveLabel(FloatField field, float initial, float current, int index, int firstOctave) {
			string initialLabel = this.FormatOctaveLabel(index, firstOctave, initial);
			field.label = current == initial
				? initialLabel
				: $"({initial}) " + this.FormatOctaveLabel(index, firstOctave, current);
		}

		private string FormatOctaveLabel(int index, int firstOctave, float value) {
			return OCTAVE_LABEL_FORMAT.WithArgs(index + firstOctave, value, Mathf.Pow(2f, index + firstOctave) * value);
		}
	}
}
