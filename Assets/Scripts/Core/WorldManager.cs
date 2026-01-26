using Cysharp.Threading.Tasks;
using NUnit.Framework;
using SoulboundBackend.Client;
using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Common;
using SoulboundBackend.Core;
using SoulboundBackend.Core.Bootstrap;
using SoulboundBackend.Core.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.LightTransport;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Zenject;
using CoroutineRunner = SoulboundBackend.Core.CoroutineRunner;
using Scene = UnityEngine.SceneManagement.Scene;

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
			int seed,
			Func<IWorldSceneRoot> rootGetter
		) {
		WorldDump? dump = serializationService.Load(world);
		if (!dump?.nonNulled ?? true) {
			dump = null;
		}
		seed = dump?.seed ?? seed;

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

		root.sceneContext.PreInstall += () => {
			root.sceneContext.Container.BindInstance(this).AsSingle();
		};
		root.sceneContext.Install();
		root.sceneContext.Resolve();

		return root.sceneContext.Container.Resolve<LevelManager>();
	}

	public void SaveWorld(string world, LevelManager levelManager) {
		SaveWorld(world, levelManager.CreateDump());
	}

	public void SaveWorld(string world, WorldDump dump) {
		serializationService.Save(dump, world);
	}

	public async UniTask TerminateWorldProcess(Scene worldScene, string world, Func<WorldDump> dumpSupplier) {
		SaveWorld(world, dumpSupplier.Invoke());
		await SceneManager.UnloadSceneAsync(worldScene).ToUniTask();
	}
}
