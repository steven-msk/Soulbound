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
	[Inject] public LevelManager? activeLevelManager { get; private set; }
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
			Action sceneLoader,
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

	IEnumerator LevelSceneLoad(Action sceneLoader, Func<IWorldSceneRoot> rootGetter, string world, WorldDump? dump, int seed) {
		sceneLoader.Invoke();
		yield return null;

		var root = rootGetter()
			?? throw new InvalidOperationException("Failed to load world scene: missing scene root");

		root.sceneContext.Install();
		root.sceneContext.Resolve();
		root.sceneContext.Container.Inject(this);

		var levelManager = root.sceneContext.Container.Resolve<LevelManager>();
		activeLevelManager = levelManager;

		var gridContext = root.CreateGridContext();

		activeLevelManager.BootstrapWorld(world, dump, seed, gridContext);

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
