using SoulboundEngine.Common;
using SoulboundEngine.Registry;
using SoulboundEngine.World.Block.State;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace SoulboundEngine.World.Block.Entity {
	public abstract class TileEntityType {
		public static readonly TileEntityType<ChestTileEntity> CHEST = Register("soulbound:chest", ITileEntityFactory.Of(ChestTileEntity.Create), new[] { Blocks.CHEST });
		public static readonly TileEntityType<SignTileEntity> SIGN = Register("soulbound:sign", ITileEntityFactory.Of(SignTileEntity.Create), new[] { Blocks.SIGN });
		[PROTOTYPICAL] public static readonly TileEntityType<SelfDestructEntity> SELF_DESTRUCT_BLOCK = Register("soulbound:self_destruct_block", ITileEntityFactory.Of(SelfDestructEntity.Create), new[] { Blocks.SELF_DESTRUCT_BLOCK });
		[PROTOTYPICAL] public static readonly TileEntityType<PulseEntity> PULSE = Register("soulbound:pulse_block", ITileEntityFactory.Of(PulseEntity.Create), new[] { Blocks.PULSE_BLOCK });

		private static TileEntityType<TE> Register<TE>(string id, ITileEntityFactory<TE> factory, Block[] blocks) where TE : TileEntity {
			Identifier identifier = Identifier.Of(id);
			RegistryKey<TileEntityType> key = RegistryKey<TileEntityType>.Of(Registries.TILE_ENTITIES.GetKey(), identifier);
			TileEntityType<TE> tileEntityType = new(key, factory, blocks.ToHashSet());
			return Registry<TileEntityType>.Register(Registries.TILE_ENTITIES, key, tileEntityType);
		}

		public static void Init() {
		}

		protected readonly RegistryKey<TileEntityType> key;
		protected readonly ITileEntityFactory factory;
		protected readonly HashSet<Block> blocks;

		protected TileEntityType(RegistryKey<TileEntityType> key, ITileEntityFactory factory, HashSet<Block> blocks) {
			this.key = key;
			this.factory = factory;
			this.blocks = blocks;
		}

		public bool Supports(BlockState blockState) {
			return this.blocks.Contains(blockState.block);
		}

		public TileEntity Instantiate(BlockPos pos, BlockState state) {
			return this.factory.Create(pos, state);
		}

		public static Identifier? GetId(TileEntityType type) {
			return Registries.TILE_ENTITIES.GetIdentifier(type);
		}

		public override string ToString() => this.key.value.ToString();

		public interface ITileEntityFactory {
			TileEntity Create(BlockPos pos, BlockState blockState);

			public static ITileEntityFactory<TE> Of<TE>(Func<BlockPos, BlockState, TE> factory) where TE : TileEntity {
				return new ITileEntityFactory<TE>.DelegateImpl(factory);
			}
		}

		public interface ITileEntityFactory<TE> : ITileEntityFactory where TE : TileEntity {
			new TE Create(BlockPos pos, BlockState blockState);

			TileEntity ITileEntityFactory.Create(BlockPos pos, BlockState blockState) {
				return this.Create(pos, blockState);
			}

			internal sealed class DelegateImpl : ITileEntityFactory<TE> {
				private readonly Func<BlockPos, BlockState, TE> factory;

				public DelegateImpl(Func<BlockPos, BlockState, TE> factory) {
					this.factory = factory;
				}

				public TE Create(BlockPos pos, BlockState blockState) {
					return this.factory(pos, blockState);
				}
			}
		}
	}

	public class TileEntityType<TE> : TileEntityType where TE : TileEntity {
		private new readonly ITileEntityFactory<TE> factory;

		public TileEntityType(RegistryKey<TileEntityType> key, ITileEntityFactory<TE> factory, HashSet<Block> blocks) 
			: base(key, factory, blocks) {
			this.factory = factory;
		}

		public new TE Instantiate(BlockPos pos, BlockState state) {
			return this.factory.Create(pos, state);
		}
	}
}
