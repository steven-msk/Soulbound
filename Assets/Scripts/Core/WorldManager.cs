using SoulboundBackend.Client.World;
using SoulboundBackend.Core;
using SoulboundBackend.Core.Bootstrap;
using SoulboundBackend.Core.Resource;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

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

    public void LoadWorld(string world) {
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
        UnityEngine.Random.InitState(seed);
        GameObject? levelManagerPrefab = ResourceManager.Get<GameObject, ResourceGroups.Runtime.Prefabs>("levelManager");
        activeLevelManager = GameObject.Instantiate(levelManagerPrefab)?.GetComponent<LevelManager>()
            ?? throw new ArgumentException("LevelManager prefab not found!");
        activeLevelManager.Init(this, world,
            GameEntryPoint.DefaultInstanceFactory(activeLevelManager),
            treeBuilder => treeBuilder.BuildTree<BootstrappableParentOfAttribute>(typeof(LevelManager))
        );
        activeLevelManager.BootstrapWorld(dump, seed);
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
