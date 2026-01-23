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
	[Obsolete]
	public event Action<LevelManager, WorldDump?>? onWorldLoaded;
	public LevelManager? activeLevelManager { get; private set; }
	private readonly WorldSerializationService serializationService;

	public WorldManager(WorldSerializationService serializationService) {
		this.serializationService = serializationService;
	}

	//public IEnumerable<string> QuerySaves() {
	//	if (!Directory.Exists(savesRoot)) {
	//		yield break;
	//	}
	//	foreach (var dir in Directory.GetDirectories(savesRoot)) {
	//		if (File.Exists(GetRegionedPath(Path.Combine(dir, LevelManager.worldDump)))) {
	//			yield return Path.GetFileName(dir);
	//		}
	//	}
	//}

	public WorldDump? LoadWorld(
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

		CoroutineRunner.GetInstance().StartCoroutine(
			LevelSceneLoad(sceneLoader, rootGetter, world, dump, seed)
		);

		return dump;
	}

	public IEnumerator LevelSceneLoad(AsyncOperation sceneLoader, Func<IWorldSceneRoot> rootGetter, string world, WorldDump? dump, int seed) {
		yield return sceneLoader;

		var root = rootGetter()
			?? throw new InvalidOperationException("Failed to load world scene: missing root");

		root.sceneContext.PreInstall += () => {
			root.sceneContext.Container.BindInstance(this).AsSingle();
		};
		root.sceneContext.Install();
		root.sceneContext.Resolve();

		var levelManager = root.sceneContext.Container.Resolve<LevelManager>();
		activeLevelManager = levelManager;

		activeLevelManager.BootstrapWorld(world, dump, seed, root.CreateGridContext());
		activeLevelManager.SpawnPlayer(dump?.player);
	}

	public void SaveWorld(string world, LevelManager levelManager) {
		SaveWorld(world, levelManager.CreateDump());
	}

	public void SaveWorld(string world, WorldDump dump) {
		serializationService.Save(dump, world);
	}

	public IEnumerator TerminateWorldProcess(Scene worldScene, string world, Func<WorldDump> dumpSupplier) {
		this.SaveWorld(world, dumpSupplier.Invoke());

		var async = SceneManager.UnloadSceneAsync(worldScene);
		yield return new WaitUntil(() => async.isDone);
		yield return null;
	}
}
