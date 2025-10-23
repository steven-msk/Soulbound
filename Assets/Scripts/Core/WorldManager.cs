using NUnit.Framework;
using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Common;
using SoulboundBackend.Core;
using SoulboundBackend.Core.Bootstrap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Zenject;
using CoroutineRunner = SoulboundBackend.Core.CoroutineRunner;
using Scene = UnityEngine.SceneManagement.Scene;

#nullable enable

public sealed class WorldManager {
	private readonly string savesRoot;
	public LevelManager? activeLevelManager { get; private set; }
	private readonly ISaveStrategy<WorldDump> saveStrategy;
	private Func<string> dataRegion;

	public WorldManager(string savesRoot, ISaveStrategy<WorldDump> saveStrategy, Func<string>? dataRegion = null) {
		this.savesRoot = savesRoot;
		this.saveStrategy = saveStrategy;
		this.dataRegion = dataRegion ?? (() => Application.persistentDataPath);
	}

	[Inject]
	public void InjectLevel(LevelManager instance) {
		activeLevelManager = instance;
	}

	public IEnumerable<string> QuerySaves() {
		if (!Directory.Exists(savesRoot)) {
			yield break;
		}
		foreach (var dir in Directory.GetDirectories(savesRoot)) {
			if (File.Exists(GetRegionedPath(Path.Combine(dir, LevelManager.worldDump)))) {
				yield return Path.GetFileName(dir);
			}
		}
	}

	public WorldDump? LoadWorld(
			string world, 
			bool initPlayerState,
			Func<SceneContext> sceneContextSupplier,
			Func<Scene> sceneSupplier
		) {
		string dumpPath = GetDumpPath(world, createIfAbsent: false);

		WorldDump? dump = saveStrategy.Load(dumpPath);
		if (!dump?.nonNulled ?? true) {
			dump = null;
		}
		int seed = dump?.seed ?? UnityEngine.Random.Range(int.MinValue, int.MaxValue);

		IEnumerator LevelSceneLoader() {
			var scene = sceneSupplier.Invoke();
			SceneManager.SetActiveScene(scene);

			var sceneContext = sceneContextSupplier.Invoke();
			yield return new WaitUntil(() => sceneContext != null && sceneContext.Container != null);

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
			if (initPlayerState) {
				activeLevelManager.SpawnPlayer(dump?.player);
			}
		}
		CoroutineRunner.GetInstance().StartCoroutine(LevelSceneLoader());

		return dump;
	}

	public void SaveWorld(string world, WorldDump dump) {
		string dumpPath = GetDumpPath(world, createIfAbsent: saveStrategy is not DoNotSaveWorldStrategy);
		saveStrategy.Save(dump, dumpPath);
		var persistent = ICachedRegistry<Block>.GetCachedRegistry().Values
			.Select(block => new KeyValuePair<Block, IBlockStateCacheStrategy>(block, block.stateCacheStrategy))
			.Where(e => e.Value is IPersistentStateCache)
			.Select(e => new KeyValuePair<Block, IPersistentStateCache>(e.Key, (IPersistentStateCache)e.Value));
        foreach (var persistentEntry in persistent) {
			persistentEntry.Value.Save(persistentEntry.Key);
        }
    }

	public IEnumerator TerminateWorldProcess(Scene worldScene, string world, Func<WorldDump> dumpSupplier) {
		this.SaveWorld(world, dumpSupplier.Invoke());

        var async = SceneManager.UnloadSceneAsync(worldScene);
        yield return new WaitUntil(() => async.isDone);
        yield return null;
    }

	public string GetDumpPath(string world, bool createIfAbsent) {
		string worldFolder = Path.Combine(savesRoot, world);
		if (createIfAbsent) {
			Directory.CreateDirectory(GetRegionedPath(worldFolder));
		}
		string dumpPath = GetRegionedPath(Path.Combine(worldFolder, LevelManager.worldDump));
		return dumpPath;
	}

	public string GetRegionedPath(params string[] paths) {
		List<string> regioned = new() { dataRegion.Invoke() };
		regioned.AddRange(paths);
		return Path.Combine(regioned.ToArray());
	}
}
