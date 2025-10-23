using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.Structure.Templates;
using SoulboundBackend.Core.Bootstrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundBackend.Core {
    public sealed class Soulbound {
        public static Soulbound instance { get; private set; } = null!;
        private readonly GameConfig config;
        public readonly WorldManager worldManager;

		public Soulbound(GameConfig config) {
            instance = this;
            this.config = config;
            ISaveStrategy<WorldDump> saveStrategy = config.dev.loadDevWorldFromSave
                ? new WorldSaveStrategy()
                : new DoNotSaveWorldStrategy();
            this.worldManager = new WorldManager(config.file.savesFolder, saveStrategy);
        }

        public void Prototype_LoadDevWorld() {
            Level.RegisterStructure(TreeStructure.instance);
            string world = config.dev.loadDevWorldFromSave
                ? config.dev.devWorld
                : $"altw_{Guid.NewGuid()}";
            worldManager.LoadWorld(world, null, GetDefaultBootstrapTree, true);
        }

        public IEnumerable<IBootstrappable> GetDefaultBootstrapTree(BootstrapTreeBuilder treeBuilder) {
            return treeBuilder.BuildTree<BootstrappableParentOfAttribute>(typeof(LevelManager));
        }

        public Level? GetActiveLevel() {
            return GetActiveLevelManager()?.Level;
        }

        public LevelManager? GetActiveLevelManager() {
            return worldManager.activeLevelManager;
        }
    }
}
