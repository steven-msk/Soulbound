namespace SoulboundEngine.UnityClient.Assets {
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using UnityEngine.AddressableAssets;
	using UnityEngine.ResourceManagement.AsyncOperations;
	using UnityEngine.ResourceManagement.ResourceLocations;

#nullable enable

	public static class AssetManager {
		const string preloadLabel = "preload";
		private static readonly ConcurrentDictionary<AssetKey, AsyncOperationHandle> assets = new();
		private static AsyncOperationHandle<IList<IResourceLocation>> locationsHandle;

		public static void LoadAllWithPreloadLabel() {
			LoadAllWithLabel(preloadLabel);
		}

		public static IEnumerable<AssetKey> LoadAllWithLabel(string label) {
			IList<IResourceLocation> locations = LoadLocations(label);
			Logger.LogInfo("Loading assets with label '{}' from {} locations", label, locations.Count);
			List<AssetKey> loadedKeys = new();

			foreach (IResourceLocation location in locations) {
				try {
					AsyncOperationHandle handle = Addressables.LoadAssetAsync<UnityEngine.Object>(location);
					handle.WaitForCompletion();

					if (handle.Status != AsyncOperationStatus.Succeeded) {
						throw handle.OperationException;
					}

					AssetKey key = new(location.PrimaryKey);
					loadedKeys.Add(FinishLoad(key, handle));
				} catch (Exception e) {
					Logger.LogFatal(e);
				}
			}

			return loadedKeys;
		}

		private static IList<IResourceLocation> LoadLocations(string label) {
			locationsHandle = Addressables.LoadResourceLocationsAsync(label);
			locationsHandle.WaitForCompletion();
			return locationsHandle.Result;
		}

		private static AssetKey FinishLoad(AssetKey key, AsyncOperationHandle handle) {
			if (!assets.TryAdd(key, handle)) {
				Logger.LogWarning("Asset already exists: {}", key);
			}
			return key;
		}

		[Obsolete]
		public static T Resolve<T>(AssetKey key) {
			if (!assets.TryGetValue(key, out AsyncOperationHandle handle)) {
				Logger.LogError("Could not find asset with key {}", key);
				return default!;
			}

			return (T)handle.Result;
		}

		public static void Shutdown() {
			locationsHandle.Release();
			foreach (AsyncOperationHandle handle in assets.Values) {
				handle.Release();
			}
		}
	}
}
