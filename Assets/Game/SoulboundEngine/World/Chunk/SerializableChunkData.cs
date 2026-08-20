using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SoulboundEngine.Client.Debug.Logging;
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
		List<JToken> tileEntities
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

			List<JToken> tileEntities = new(chunk.GetTileEntityPositions().Count);
			foreach (var blockPos in chunk.GetTileEntityPositions()) {
				JToken? json = chunk.GetTileEntityJsonForSaving(blockPos);
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

			List<JToken> tileEntities = new();
			JArray tileEntitiesArray = (JArray)jsonObject["tileEntities"]!;
			foreach (JToken token in tileEntitiesArray) {
				tileEntities.Add(token);
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

			WorldChunk chunk = new(level, chunkPos, sections, containerFactory);

			foreach (var token in this.tileEntities) {
				try {
					BlockPos? blockPos = TileEntity.GetPosFromJson(token);
					if (blockPos is not { } pos) {
						Logger.LogError("Failed to parse TileEntity block pos: {}", token);
						continue;
					}
					ChunkSection section = sections[level.GetSectionIndexFromBlock(pos.y)];
					SectionPos sectionPos = ChunkSection.ComputeLocalPos(pos.x, pos.y);
					BlockState state = section.GetBlockState(sectionPos.x, sectionPos.y);
					TileEntity? tileEntity = TileEntity.FromJson(token, pos, state);
					if (tileEntity != null) {
						chunk.SetTileEntity(tileEntity);
					}
				} catch (Exception e) {
					Logger.LogFatal(e);
				}
			}
			chunk.SyncBlocksWithTileEntities();

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
