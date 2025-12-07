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
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Zenject;
using CoroutineRunner = SoulboundBackend.Core.CoroutineRunner;
using Scene = UnityEngine.SceneManagement.Scene;

#nullable enable

public sealed class WorldManager {
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
			Func<SceneContext> sceneContextSupplier,
			Action sceneLoader
		) {
		WorldDump? dump = serializationService.Load(world);
		if (!dump?.nonNulled ?? true) {
			dump = null;
		}
		int seed = dump?.seed ?? UnityEngine.Random.Range(int.MinValue, int.MaxValue);

		IEnumerator LevelSceneLoader() {
			sceneLoader.Invoke();
			yield return null;

			var sceneContext = sceneContextSupplier.Invoke();
			yield return new WaitUntil(() => sceneContext.HasInstalled && sceneContext.HasResolved);

			sceneContext.Container.Inject(this);
			if (activeLevelManager == null) {
				throw new InvalidOperationException("Level Manager is not injected");
			}

			LevelGridContext gridContext;
			GameObject grid = GameObject.Find("Grid");
			if (grid == null) {
				gridContext = LevelGridContext.FromRuntimePrefabs();
			} else {
				Tilemap tilemap = grid.GetComponentInChildren<Tilemap>();
				gridContext = new LevelGridContext(grid.GetComponent<Grid>(), tilemap);
			}

			activeLevelManager.BootstrapWorld(world, dump, seed, gridContext);
			onWorldLoaded?.Invoke(activeLevelManager, dump);
			
		}
		CoroutineRunner.GetInstance().StartCoroutine(LevelSceneLoader());

		return dump;
	}

	public void SaveWorld(string world, LevelManager levelManager) {
		this.SaveWorld(world, levelManager.CreateDump());
	}

	public void SaveWorld(string world, WorldDump dump) {
		serializationService.Save(dump, world);

		//var persistent = ICachedRegistry<Block>.GetCachedRegistry().Values
		//	.Select(block => new KeyValuePair<Block, IBlockStateCacheStrategy>(block, block.stateCacheStrategy))
		//	.Where(e => e.Value is IPersistentStateCache)
		//	.Select(e => new KeyValuePair<Block, IPersistentStateCache>(e.Key, (IPersistentStateCache)e.Value));

		//foreach (var persistentEntry in persistent) {
		//	persistentEntry.Value.Save(persistentEntry.Key);
		//}
	}

	public IEnumerator TerminateWorldProcess(Scene worldScene, string world, Func<WorldDump> dumpSupplier) {
		this.SaveWorld(world, dumpSupplier.Invoke());

		var async = SceneManager.UnloadSceneAsync(worldScene);
		yield return new WaitUntil(() => async.isDone);
		yield return null;
	}
}
