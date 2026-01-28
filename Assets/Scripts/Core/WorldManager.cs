using Cysharp.Threading.Tasks;
using SoulboundBackend.Client.World;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#nullable enable

public sealed class WorldManager {
	public LevelManager? activeLevelManager { get; private set; }
	private readonly WorldSerializationService serializationService;

	public WorldManager(WorldSerializationService serializationService) {
		this.serializationService = serializationService;
	}

	public IEnumerable<string> ListSaves() {
		string savesRoot = serializationService.GetSavesRoot();
		if (!Directory.Exists(savesRoot)) {
			yield break;
		}

		foreach (var dir in Directory.GetDirectories(savesRoot)) {
			if (File.Exists(Path.Combine(dir, LevelManager.worldDump))) {
				yield return Path.GetFileName(dir);
			}
		}
	}

	public async UniTask<WorldDump?> LoadWorld(
			string world,
			AsyncOperation sceneLoader,
			Func<IWorldSceneRoot> rootGetter
		) {
		if (!serializationService.Load(world, out WorldDump? dump)) {
			throw new ArgumentException("World not found: " + world);
		}
		var seed = dump.GetValueOrDefault().seed;

		var levelManager = await LoadWorldSceneAsync(sceneLoader, rootGetter);
		activeLevelManager = levelManager;

		activeLevelManager.BootstrapWorld(world, dump, seed, rootGetter().CreateGridContext());
		activeLevelManager.SpawnPlayer(dump?.player);
		
		return dump;
	}

	public async UniTask<LevelManager> LoadWorldSceneAsync(AsyncOperation sceneLoader, Func<IWorldSceneRoot> rootGetter) {
		await sceneLoader.ToUniTask();

		var root = rootGetter()
			?? throw new InvalidOperationException("Failed to load world scene: missing root");

		root.sceneContext.Install();
		root.sceneContext.Resolve();

		return root.sceneContext.Container.Resolve<LevelManager>();
	}

	public void SaveWorld(LevelManager levelManager) {
		serializationService.Save(levelManager.CreateDump(), levelManager.world);
	}

	public void QuitActiveSession() {
		if (activeLevelManager == null) {
			return;
		}

		activeLevelManager.StopSession();
		SaveWorld(activeLevelManager);
	}

	public bool IsSessionActive() => activeLevelManager != null;
}
