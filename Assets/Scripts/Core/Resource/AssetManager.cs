using SoulboundBackend.Core.AssetManagement;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Logger = SoulboundBackend.Core.Debug.Logging.Logger;

#nullable enable

namespace SoulboundBackend.Core.Resource {
	public static class AssetManager {
		const string preloadLabel = "preload";
		private static readonly ConcurrentDictionary<AssetKey, AsyncOperationHandle> assets = new();
		private static AsyncOperationHandle<IList<IResourceLocation>> locationsHandle;

		public static void PreloadAll() {
			var locations = LoadLocations();
			Logger.LogInfo("Preloading assets from {} locations", locations.Count());

			foreach (var location in locations) {
				var handle = Addressables.LoadAssetAsync<UnityEngine.Object>(location);
				handle.WaitForCompletion();

				if (handle.Status != AsyncOperationStatus.Succeeded) {
					throw handle.OperationException;
				}

				FinishPreload(new AssetKey(location.PrimaryKey), handle);
			}
		}

		private static IList<IResourceLocation> LoadLocations() {
			locationsHandle = Addressables.LoadResourceLocationsAsync(preloadLabel);
			locationsHandle.WaitForCompletion();
			return locationsHandle.Result;
		}

		private static void FinishPreload(AssetKey key, AsyncOperationHandle handle) {
			if (!assets.TryAdd(key, handle)) {
				Logger.LogWarning("Failed to preload asset: key already exists '{}'", key);
			}
		}

		public static T Resolve<T>(AssetKey key) where T : UnityEngine.Object {
			if (!assets.TryGetValue(key, out var handle)) {
				throw new InvalidOperationException($"Could not find asset with key '{key}'");
			}

			return (T)handle.Result;
		}

		public static void Shutdown() {
			locationsHandle.Release();
			foreach (var handle in assets.Values) {
				handle.Release();
			}
		}
	}
}
