using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.World.Serialization;
using System;
using System.Collections.Generic;

#nullable enable

namespace SoulboundEngine.Client.World {
	using File = Serialization.File;

	public sealed class WorldSavesManager {
		private readonly File root;
		private readonly HashSet<string> newWorlds = new();

		public WorldSavesManager(File root) {
			this.root = root.EnsureExists();
		}

		public IEnumerable<WorldSave> ListSaves(IWorldSaveValidator saveValidator) {
			foreach (var file in this.root.ListFiles()) {
				if (saveValidator.Validate(file, out int seed, out string worldName, out File chunksFolder)) {
					yield return new WorldSave(file, chunksFolder, worldName, seed, this.IsNew(worldName));
				}
			}
		}

		public WorldSave GetSave(string world, IWorldSaveValidator saveValidator) {
			foreach (var save in this.ListSaves(saveValidator)) {
				if (save.name == world) return save;
			}
			throw new ArgumentException("World not found: " + world);
		}

		public void OnWorldEntered(string world) {
			this.newWorlds.Remove(world);
		}

		public void CreateNewWorld(string world, int seed, IWorldSaveValidator saveValidator) {
			File saveDirectory = this.ToSaveDirectory(world);
			if (!saveDirectory.Mkdir()) {
				Logger.LogError("Failed to create world: {}", world);
				return;
			}

			this.newWorlds.Add(world);
			saveValidator.ValidateNewSave(saveDirectory, seed);
		}

		public void DeleteWorld(string world) {
			File saveDirectory = this.ToSaveDirectory(world);
			if (!saveDirectory.Delete()) {
				Logger.LogError("Could not delete world: {}", world);
			}
		}

		public File ToSaveDirectory(WorldSave save) => this.ToSaveDirectory(save.name);

		public File ToSaveDirectory(string world) {
			return this.root.Combine(world);
		}

		public bool IsNew(string world) => this.newWorlds.Contains(world);
	}
}
