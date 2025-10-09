using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Logger = SoulboundBackend.Common.Logging.Logger;
using SoulboundBackend.Common.Logging;
using System.Collections.Concurrent;
using System.IO;

namespace SoulboundBackend.Core.Resource {

	[CreateAssetMenu(menuName = "Resource Group")]
	public class ResourceGroup : ScriptableObject {
		private static readonly Logger logger = Logger.CreateInstance();
		public string groupAddress = "";
		public string searchFolder = "Assets/Resources/";
		public string assetType = "";
		private ConcurrentDictionary<string, ResourceEntry> cached = new();
		[SerializeField] private string extension;
		[SerializeField] private string[] resourceAddresses;

#nullable enable

#if UNITY_EDITOR
		[ContextMenu("Refresh Group")]
		public void RefreshGroup() {
			var guids = AssetDatabase.FindAssets($"t:{assetType}", new[] { searchFolder });

			resourceAddresses = guids
                .Select(g => AssetDatabase.GUIDToAssetPath(g))
                .Where(path => Path.GetDirectoryName(path).Replace("\\", "/") == searchFolder)
                .ToArray();
			if (resourceAddresses != null && resourceAddresses.Length > 0) {
				extension = Path.GetExtension(resourceAddresses[0]);
			}

            EditorUtility.SetDirty(this);
		}
#endif

		public TAsset? GetAsset<TAsset>(string name) where TAsset : UnityEngine.Object {
			string path = $"{searchFolder}/{name}{extension}";

			if (!AssetDatabase.AssetPathExists(path)) {
				logger.LogError(LogModules.resource, "Could not find asset '{}' of type {} in resource group '{}'", name, typeof(TAsset), groupAddress);
				return null;
			}

			if (cached.TryGetValue(path, out var cachedEntry)) {
				return (TAsset?)cachedEntry.resource;
			}

			TAsset asset = AssetDatabase.LoadAssetAtPath<TAsset>(path);
			ResourceEntry entry = new(asset, name, path);
			cached[path] = entry;

			return asset;
		}

		static ResourceGroup() {
			logger.LogInfo(LogModules.resource, "ResourceGroup type loaded");
		}
	}
}

