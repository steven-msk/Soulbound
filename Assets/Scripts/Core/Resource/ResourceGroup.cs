using SoulboundBackend.Common;
using SoulboundBackend.Common.Logging;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Logger = SoulboundBackend.Common.Logging.Logger;

namespace SoulboundBackend.Core.Resource {

	[CreateAssetMenu(menuName = "Resource Group")]
	[PROTOTYPICAL]
	[Obsolete]
	public class ResourceGroup : ScriptableObject {
		private static readonly Logger logger = Logger.CreateInstance();
		public string groupAddress = "";
		public string searchFolder = "Assets/Resources/";
		public string assetType = "";
		private ConcurrentDictionary<string, ResourceEntry> cached = new();
		private FileSystemWatcher fileWatcher;
		private bool justRefreshed = false;
		[SerializeField] private string extension;
		[SerializeField] private bool issueAutomaticRefreshes = true;
		[SerializeField] private string[] resourceAddresses;

#nullable enable

#if UNITY_EDITOR
		[ContextMenu("Refresh Addresses")]
		public void RefreshAddresses() {
			var guids = AssetDatabase.FindAssets($"t:{assetType}", new[] { searchFolder });

			resourceAddresses = guids
				.Select(g => AssetDatabase.GUIDToAssetPath(g))
				.Where(path => Path.GetDirectoryName(path).Replace("\\", "/") == searchFolder)
				.ToArray();
			if (resourceAddresses != null && resourceAddresses.Length > 0) {
				extension = Path.GetExtension(resourceAddresses[0]);
			}

			logger.LogInfo(LogModules.resource, "Found {} resources in folder {}", resourceAddresses?.Length ?? 0, searchFolder);

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

		private void OnEnable() {
			fileWatcher = new FileSystemWatcher(searchFolder) {
				IncludeSubdirectories = false,
				EnableRaisingEvents = true,
			};
			FileSystemEventHandler delayedRefresh = (sender, e) => {
				if (!issueAutomaticRefreshes || justRefreshed) {
					return;
				}
				justRefreshed = true;

				EditorApplication.delayCall += () => {
					logger.LogInfo(LogModules.resource,
						"Issued an automatic refresh for group with address '{}'" +
						" from source folder '{}'", groupAddress, searchFolder);
#if UNITY_EDITOR
					RefreshAddresses();
#endif
					justRefreshed = false;
				};
			};

			fileWatcher.Created += delayedRefresh;
			fileWatcher.Renamed += (sender, e) => delayedRefresh.Invoke(sender, e);
			fileWatcher.Deleted += delayedRefresh;
		}

		static ResourceGroup() {
			logger.LogInfo(LogModules.resource, "ResourceGroup type loaded");
		}
	}
}

