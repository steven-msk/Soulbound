using NUnit.Framework;
using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class WorldSaveStrategyTests {
	private string CreateTempDir() {
		string path = Path.Combine(Path.GetTempPath(), "worldSaveStrategyTests_" + Guid.NewGuid());
		Directory.CreateDirectory(path);
		return path;
	}

	[Test]
	public void GetDumpPath_CreatesFolder_AndReturnsCorrectFilePath() {
		string root = CreateTempDir();
		string dataPath = Application.temporaryCachePath;
		string worldName = "testWorld_" + Guid.NewGuid();

		var strategy = new WorldSaveStrategy(root, dataPath);

		string dumpPath = strategy.GetDumpPath(worldName);

		string expectedDir = Path.Combine(dataPath, root, worldName);
		string expectedFile = Path.Combine(dataPath, root, worldName, LevelManager.worldDump);

		Assert.AreEqual(expectedFile, dumpPath);
		Assert.True(Directory.Exists(expectedDir));
	}

	[Test]
	public void Save_WritesFileCorrectly() {
		string root = CreateTempDir();
		string dataPath = Application.temporaryCachePath;
		string worldName = "world_" + Guid.NewGuid();

		var strategy = new WorldSaveStrategy(root, dataPath);

		WorldDump dump = new() {
			seed = 123,
			generatedChunks = new WorldChunk[0],
			//structurePlacements = new()
		};

		string dumpPath = strategy.GetDumpPath(worldName);
		strategy.Save(dump, worldName);

		Assert.True(File.Exists(dumpPath));

		string json = File.ReadAllText(dumpPath);
		var loaded = JsonConvert.DeserializeObject<WorldDump>(json, Soulbound.globalJsonSettings);

		Assert.AreEqual(123, loaded.seed);
	}

	[Test]
	public void Load_ReturnsNull_WhenFileDoesNotExist() {
		string root = CreateTempDir();
		string dataPath = CreateTempDir();
		var strategy = new WorldSaveStrategy(root, dataPath);

		var result = strategy.Load("non_existent_world_xd");

		Assert.Null(result);
	}

	[Test]
	public void Load_ReturnsSavedDump() {
		string root = CreateTempDir();
		string dataPath = CreateTempDir();
		string worldName = "world_" + Guid.NewGuid();

		var strategy = new WorldSaveStrategy(root, dataPath);

		WorldDump dump = new() {
			seed = 999,
			generatedChunks = new WorldChunk[0],
			//structurePlacements = new()
		};

		strategy.Save(dump, worldName);

		WorldDump? loaded = strategy.Load(worldName);

		Assert.NotNull(loaded);
		Assert.AreEqual(999, loaded?.seed);
	}

	//[Test]
	//public void GetDataPath_JoinsPathsCorrectly() {
	//	var strategy = new WorldSaveStrategy("root", "data");

	//	string joined = strategy.DataPath("folderA", "file.txt");

	//	Assert.AreEqual(Path.Combine("data", "folderA", "file.txt"), joined);
	//}
}
