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
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using CoroutineRunner = SoulboundBackend.Core.CoroutineRunner;

#nullable enable

public sealed class WorldManager {
	private readonly string savesRoot;
	private LevelManager? activeLevelManager;

	public WorldManager(string savesRoot) {
		this.savesRoot = savesRoot;
	}

	public IEnumerable<string> QuerySaves() {
		if (!Directory.Exists(savesRoot)) {
			yield break;
		}
		foreach (var dir in Directory.GetDirectories(savesRoot)) {
			if (File.Exists(GetPersistentPath(Path.Combine(dir, LevelManager.worldDump)))) {
				yield return Path.GetFileName(dir);
			}
		}
	}

	public void LoadWorld(string world, Scene? levelScene) {
		string dumpPath = GetDumpPath(world);

		WorldDump? dump = null;
		int seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);

		if (File.Exists(dumpPath)) {
			dump = JsonConvert.DeserializeObject<WorldDump>(
				File.ReadAllText(dumpPath),
				LevelManager.globalJsonSettings
			);
			seed = dump?.seed ?? seed;
		}

		GameObject? levelManagerPrefab = ResourceManager.Get<GameObject, ResourceGroups.Runtime.Prefabs>("levelManager");
		BootstrappableInstanceFactory instanceFactory = new();
		LevelManager InstantiateLevelManager() {
			return GameObject.Instantiate(levelManagerPrefab)?.GetComponent<LevelManager>()
				?? throw new ArgumentException("LevelManager prefab not found!");
		}
		void FinalizeLevelManager() {
            if (activeLevelManager == null) {
                throw new InvalidOperationException("LevelManager instantiation failed.");
            }

            activeLevelManager.Init(this, world, instanceFactory,
                treeBuilder => treeBuilder.BuildTree<BootstrappableParentOfAttribute>(typeof(LevelManager))
            );

            Grid grid = GameObject.Find("Grid").GetComponent<Grid>();
            Tilemap tilemap = grid.GetComponentInChildren<Tilemap>();

            activeLevelManager.BootstrapWorld(dump, seed,
                new LevelGridContext(grid, tilemap)
            );
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
			instanceFactory = GameEntryPoint.DefaultInstanceFactory(activeLevelManager);
			FinalizeLevelManager();
		}
	}

	public void SaveWorld(string world, WorldDump dump) {
		string dumpPath = GetDumpPath(world);
		string json = JsonConvert.SerializeObject(dump, LevelManager.globalJsonSettings); 

		File.WriteAllText(dumpPath, json);
	}

	private string GetDumpPath(string world) {
		string worldFolder = Path.Combine(savesRoot, world);
		Directory.CreateDirectory(GetPersistentPath(worldFolder));
		string dumpPath = GetPersistentPath(Path.Combine(worldFolder, LevelManager.worldDump));
		return dumpPath;
	}

	private string GetPersistentPath(string path) {
		return Path.Combine(Application.persistentDataPath, path);
	}
}
