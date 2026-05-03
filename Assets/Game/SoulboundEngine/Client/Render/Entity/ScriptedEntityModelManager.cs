using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Core.Registry;
using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

#nullable enable

namespace SoulboundEngine.Client.Render.Entity {
	public sealed class ScriptedEntityModelManager : IDisposable {
		private readonly string assetLabel;
		private readonly List<AsyncOperationHandle<ScriptedEntityModel>> handles = new();
		private readonly Dictionary<Identifier, ScriptedEntityModel> modelById = new();
		private AsyncOperationHandle<IList<IResourceLocation>> locationsHandle;

		public ScriptedEntityModelManager(string assetLabel) {
			this.assetLabel = assetLabel;
		}

		public void LoadAll() {
			IList<IResourceLocation> locations = this.GetLocations();

			foreach (var location in locations) {
				AsyncOperationHandle<ScriptedEntityModel> handle = Addressables.LoadAssetAsync<ScriptedEntityModel>(location);
				handle.WaitForCompletion();

				if (handle.Status != AsyncOperationStatus.Succeeded) {
					Logger.LogFatal(handle.OperationException);
					continue;
				}

				this.handles.Add(handle);
				ScriptedEntityModel model = handle.Result;
				Identifier id = model.GetIdentifier();

				if (!this.modelById.TryAdd(id, model)) {
					Logger.LogWarning("Scripted entity model already present: {}", id);
				}
			}
		}

		private IList<IResourceLocation> GetLocations() {
			this.locationsHandle = Addressables.LoadResourceLocationsAsync(this.assetLabel, typeof(ScriptedEntityModel));
			this.locationsHandle.WaitForCompletion();
			return this.locationsHandle.Result;
		}

		public ScriptedEntityModel? Get(Identifier identifier) {
			if (this.modelById.TryGetValue(identifier, out ScriptedEntityModel model)) {
				return model;
			} else {
				Logger.LogError("Could not find scripted entity model for '{}'", identifier);
				return null;
			}
		}

		public void Dispose() {
			this.locationsHandle.Release();
			foreach (var handle in this.handles) {
				handle.Release();
			}
		}
	}
}
