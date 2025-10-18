using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.Structure.Templates;
using SoulboundBackend.Core.Bootstrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core {
    public sealed class Soulbound {
        public static Soulbound instance { get; private set; }
        private readonly GameConfig config;
        private readonly WorldManager worldManager;

        public Soulbound(GameConfig config) {
            instance = this;
            this.config = config;
            this.worldManager = new WorldManager(config.file.savesFolder, new WorldSaveStrategy());
        }

        public void Prototype_LoadDevWorld() {
            Level.RegisterStructure(TreeStructure.instance);
            string world = config.dev.loadDevWorldFromSave ? config.dev.devWorld : null;
            worldManager.LoadWorld(world, null, GetDefaultBootstrapTree, true);
        }

        public IEnumerable<IBootstrappable> GetDefaultBootstrapTree(BootstrapTreeBuilder treeBuilder) {
            return treeBuilder.BuildTree<BootstrappableParentOfAttribute>(typeof(LevelManager));
        }
    }
}
