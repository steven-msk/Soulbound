using SoulboundBackend.Client;
using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Client.World;
using SoulboundBackend.Core;
using SoulboundBackend.Core.Bootstrap;
using SoulboundBackend.Core.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
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
			Scene? levelScene, 
			Func<BootstrapTreeBuilder, IEnumerable<IBootstrappable>> bootstrapTreeFunc,
			bool initPlayerState
		) {
		string dumpPath = GetDumpPath(world, createIfAbsent: false);

		WorldDump? dump = saveStrategy.Load(dumpPath);
		if (!dump?.nonNulled ?? true) {
			dump = null;
		}
		int seed = dump?.seed ?? UnityEngine.Random.Range(int.MinValue, int.MaxValue);

		GameObject? levelManagerPrefab = ResourceManager.Get<GameObject, ResourceGroups.Runtime.Prefabs>("levelManager");
		BootstrappableInstanceFactory instanceFactory = new();
		LevelManager InstantiateLevelManager() {
			return GameObject.Instantiate(levelManagerPrefab)?.GetComponent<LevelManager>()
				?? throw new ArgumentException("LevelManager prefab not found!");
		}
		void FinalizeLevelManager() {
            if (activeLevelManager == null) {
                throw new InvalidOperationException("LevelManager not instantiated.");
            }

            activeLevelManager.Init(this, world, instanceFactory, bootstrapTreeFunc);

			LevelGridContext gridContext;
			GameObject grid = GameObject.Find("Grid");
			if (grid == null) {
				gridContext = LevelGridContext.FromRuntimePrefabs();
			} else {
				Tilemap tilemap = grid.GetComponentInChildren<Tilemap>();
				gridContext = new LevelGridContext(grid.GetComponent<Grid>(), tilemap);
			}

			activeLevelManager.BootstrapWorld(dump, seed, gridContext);
			if (initPlayerState) {
				activeLevelManager.SpawnPlayer(dump?.player); 
			}
        }

		if (levelScene == null) {
			T FindByType<T>() where T : UnityEngine.Object {
				return GameObject.FindAnyObjectByType<T>()
					?? throw new ArgumentException($"No object of type {typeof(T)} found in the current scene!");
			}

			IEnumerator LevelSceneLoader() {
				AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("WorldScene");
				yield return new WaitUntil(() => asyncLoad.isDone);

				activeLevelManager = InstantiateLevelManager(); 
				var playerPrefab = ResourceManager.Get<GameObject, ResourceGroups.Runtime.Prefabs>("player");
				var player = GameObject.Instantiate(playerPrefab)!.GetComponent<PlayerController>();

				instanceFactory.Register<LevelManager>(() => activeLevelManager);
				instanceFactory.Register<PlayerController>(() => player);
				instanceFactory.Register<InventoryController>(FindByType<InventoryController>);
				instanceFactory.Register<HotbarController>(FindByType<HotbarController>);
				instanceFactory.Register<PlayerPhysics>(() => player.GetComponent<PlayerPhysics>()); 
				instanceFactory.Register<InputHandler>(FindByType<InputHandler>);
				instanceFactory.Register<ItemUsageHandler>(() => new ItemUsageHandler(player));

				FinalizeLevelManager();
			}
			CoroutineRunner.GetInstance().StartCoroutine(LevelSceneLoader());
		} else {
			SceneManager.SetActiveScene(levelScene.Value);
			activeLevelManager = InstantiateLevelManager();
			instanceFactory = GameEntryPoint.DefaultInstanceFactory().Invoke(activeLevelManager);
			FinalizeLevelManager();
		}
		return dump;
	}

	public void SaveWorld(string world, WorldDump dump) {
		string dumpPath = GetDumpPath(world, createIfAbsent: saveStrategy is not DoNotSaveWorldStrategy);
		saveStrategy.Save(dump, dumpPath);
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
