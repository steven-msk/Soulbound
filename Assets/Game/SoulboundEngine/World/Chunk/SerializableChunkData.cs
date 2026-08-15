using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Registry;
using SoulboundEngine.World.Block;
using SoulboundEngine.World.Block.Entity;
using SoulboundEngine.World.Block.State;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace SoulboundEngine.World.Chunk {
	using Block = Block.Block;
	using Level = Level.Level;

	public record SerializableChunkData(
		ChunkPos chunkPos,
		List<SerializableChunkData.SectionData> sectionData,
		List<JObject> tileEntities
	) {
		public static SerializableChunkData Of(Level level, Chunk chunk) {
			if (!chunk.CanBeSerialized()) {
				throw new ArgumentException("Chunk cant be serialized: " + chunk);
			}

			ChunkPos pos = chunk.GetPos();
			List<SectionData> sectionData = new();
			ChunkSection[] sections = chunk.GetSections();

			for (int sectionY = level.GetBottomSectionY(); sectionY < level.GetTopSectionY(); sectionY++) {
				int sectionIndex = chunk.GetSectionIndexFromSectionY(sectionY);
				if (sectionIndex >= 0 && sectionIndex < sections.Length) {
					ChunkSection section = sections[sectionIndex];
					sectionData.Add(new SectionData(sectionY, section));
				}
			}

			List<JObject> tileEntities = new(chunk.GetTileEntityPositions().Count);
			foreach (var blockPos in chunk.GetTileEntityPositions()) {
				JObject? json = chunk.GetTileEntityJsonForSaving(blockPos);
				if (json != null) tileEntities.Add(json);
			}

			return new SerializableChunkData(pos, sectionData, tileEntities);
		}

		public static SerializableChunkData Parse(string jsonString, Level level) {
			JObject jsonObject = JObject.Parse(jsonString);
			ChunkPos chunkPos = ChunkPos.Parse((string)jsonObject["pos"]!);

			List<SectionData> sectionData = new();
			JObject sections = (JObject)jsonObject["sections"]!;

			foreach (JProperty sectionProp in sections.Properties()) {
				int sectionY = int.Parse(sectionProp.Name);
				JArray states = (JArray)sectionProp.Value;

				BlockStateContainer container = level.BlockStateContainerFactory()();
				int i = 0;
				container.ForEachBlock((x, y, _) => {
					int stateId = states[i].Value<int>();
					container.Set(x, y, Block.GetState(stateId));
					i++;
				});

				sectionData.Add(new SectionData(sectionY, new ChunkSection(container)));
			}

			List<JObject> tileEntities = new();
			JArray tileEntitiesArray = (JArray)jsonObject["tileEntities"]!;
			foreach (JToken token in tileEntitiesArray) {
				tileEntities.Add((JObject)token);
			}

			return new SerializableChunkData(chunkPos, sectionData, tileEntities);
		}

		public Chunk Read(Level level, ChunkPos chunkPos) {
			if (!this.chunkPos.Equals(chunkPos)) {
				Logger.LogError("Chunk {} is in the wrong location: expected {}, got {}", chunkPos, chunkPos, this.chunkPos);
			}

			int sectionCount = level.GetSectionCount();
			ChunkSection[] sections = new ChunkSection[sectionCount];
			Func<BlockStateContainer> containerFactory = level.BlockStateContainerFactory();

			foreach (var section in this.sectionData) {
				if (section.chunkSection != null) {
					sections[level.GetSectionIndexFromSectionY(section.y)] = section.chunkSection;
				}
			}

			Chunk chunk = new WorldChunk(level, chunkPos, sections, containerFactory);

			foreach (var token in this.tileEntities) {
				try {
					Identifier typeId = Identifier.Of((string)token["type"]!);
					TileEntityType type = Registries.TILE_ENTITIES.Get(typeId);

					BlockPos blockPos = BlockPos.Parse((string)token["pos"]!);
					ChunkSection section = sections[level.GetSectionIndexFromBlock(blockPos.y)];
					SectionPos sectionPos = ChunkSection.ComputeLocalPos(blockPos.x, blockPos.y);
					BlockState state = section.GetBlockState(sectionPos.x, sectionPos.y);

					TileEntity tileEntity = type.Instantiate(blockPos, state);
					tileEntity.Read(token);
					chunk.SetTileEntity(tileEntity);
				} catch (Exception e) {
					Logger.LogFatal(e);
				}
			}

			return chunk;
		}

		public string Write() {
			JObject sections = new();
			foreach (var section in this.sectionData) {
				if (section.chunkSection != null) {
					int sectionY = section.y;
					IEnumerable<int> states = section.chunkSection.GetStatesImmutable().Select(Block.GetRawID);
					JArray array = new(states);
					sections[sectionY.ToString()] = array;
				}
			}

			JArray tileEntities = new();
			foreach (var tileEntity in this.tileEntities) {
				tileEntities.Add(tileEntity);
			}

			JObject json = new() {
				["pos"] = this.chunkPos.ToString(),
				["sections"] = sections,
				["tileEntities"] = tileEntities
			};
			return json.ToString(Formatting.None);
		}

		public record SectionData(int y, ChunkSection? chunkSection);
	}
}
