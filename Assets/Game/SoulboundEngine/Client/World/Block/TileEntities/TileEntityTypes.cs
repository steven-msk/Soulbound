using SoulboundEngine.Core.Registry;

namespace SoulboundEngine.Client.World.BlockSystem.TileEntities {
	public static class TileEntityTypes {
		public static readonly TileEntityType<ChestTileEntity> CHEST = Register(Blocks.CHEST, new TileEntityType<ChestTileEntity>());
		public static readonly TileEntityType<SelfDestructEntity> SELF_DESTRUCT_BLOCK = Register(Blocks.SELF_DESTRUCT_BLOCK, new TileEntityType<SelfDestructEntity>());
		public static readonly TileEntityType<PulseEntity> PULSE = Register(Blocks.PULSE_BLOCK, new TileEntityType<PulseEntity>());
		public static readonly TileEntityType<ObjectTileEntity> OBJECT = Register(Blocks.AREA_TRIGGER_BLOCK, new TileEntityType<ObjectTileEntity>());

		public static TET Register<TET>(string id, TET tileEntityType) where TET : TileEntityType {
			return Register(KeyOf(id), tileEntityType);
		}

		public static TET Register<TET>(Identifier identifier, TET tileEntityType) where TET : TileEntityType {
			return Register(KeyOf(identifier), tileEntityType);
		}

		public static TET Register<TET>(RegistryKey<TileEntityType> key, TET tileEntityType) where TET : TileEntityType {
			return Registry<TileEntityType>.Register(Registries.TILE_ENTITIES, key, tileEntityType);
		}

		public static TET Register<TET>(Block targetBlock, TET tileEntityType) where TET : TileEntityType {
			return Register(Blocks.GetIdentifier(targetBlock), tileEntityType);
		}

		private static RegistryKey<TileEntityType> KeyOf(string id) {
			return KeyOf(Identifier.Of(id));
		}

		private static RegistryKey<TileEntityType> KeyOf(Identifier identifier) {
			return RegistryKey<TileEntityType>.Of(Registries.TILE_ENTITIES.GetKey(), identifier);
		}

		public static Identifier GetIdentifier<TET>(TET tileEntityType) where TET : TileEntityType {
			return Registries.TILE_ENTITIES.GetIdentifier(tileEntityType);
		}

		public static void Init() {
		}
 	}
}
