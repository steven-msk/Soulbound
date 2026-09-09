namespace SoulboundEngine.World {
	using SoulboundEngine.Serialization;
	using SoulboundEngine.World.Gen;
	using SoulboundEngine.World.Serialization;
	using System;
	using System.Collections.Generic;

#nullable enable

	public sealed class WorldSavesManager {
		private readonly File root;
		private readonly HashSet<string> newWorlds = new();

		public WorldSavesManager(File root) {
			this.root = root.EnsureExists();
		}

		public IEnumerable<WorldSave> ListSaves(IWorldSaveValidator saveValidator) {
			foreach (File file in this.root.ListFiles()) {
				if (saveValidator.Validate(file) is WorldSave save) {
					save.isNew = this.IsNew(save.name);
					yield return save;
				}
			}
		}

		public WorldSave GetSave(string world, IWorldSaveValidator saveValidator) {
			foreach (WorldSave save in this.ListSaves(saveValidator)) {
				if (save.name == world) return save;
			}
			throw new ArgumentException("World not found: " + world);
		}

		public void OnWorldEntered(string world) {
			this.newWorlds.Remove(world);
		}

		public void CreateNewWorld(string world, int seed, WorldPreset preset, IWorldSaveValidator saveValidator) {
			File saveDirectory = this.ToSaveDirectory(world);
			if (!saveDirectory.Mkdir()) {
				Logger.LogError("Failed to create world: {}", world);
				return;
			}

			this.newWorlds.Add(world);
			saveValidator.ValidateNewSave(saveDirectory, seed, preset);
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
