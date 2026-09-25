namespace SoulboundEngine.World.Chunk {
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using SoulboundEngine.Registry;
	using SoulboundEngine.Serialization;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Block.Entity;
	using SoulboundEngine.World.Block.State;
	using SoulboundEngine.World.Gen;
	using SoulboundEngine.World.Gen.Biome;
	using SoulboundEngine.World.Level;
	using System;
	using System.Collections.Generic;
	using System.Linq;

#nullable enable

	public record SerializableChunkData(
		int sinceVersion,
		ChunkPos chunkPos,
		int[]? heightmap,
		List<SerializableChunkData.SectionData> sectionData,
		List<JToken> tileEntities,
		RegistryEntry<Biome>[] biomes
	) {
		public static SerializableChunkData Of(Chunk chunk, int version) {
			if (!chunk.CanBeSerialized()) {
				throw new ArgumentException("Chunk cant be serialized: " + chunk);
			}

			ChunkPos pos = chunk.pos;
			List<SectionData> sectionData = new();
			ChunkSection[] sections = chunk.GetSections();

			for (int sectionY = chunk.GetBottomSectionY(); sectionY < chunk.GetTopSectionY(); sectionY++) {
				int sectionIndex = chunk.GetSectionIndexFromSectionY(sectionY);
				if (sectionIndex >= 0 && sectionIndex < sections.Length) {
					ChunkSection section = sections[sectionIndex];
					sectionData.Add(new SectionData(sectionY, section));
				}
			}

			List<JToken> tileEntities = new(chunk.GetTileEntityPositions().Count);
			foreach (BlockPos blockPos in chunk.GetTileEntityPositions()) {
				JToken? json = chunk.GetTileEntityJsonForSaving(blockPos);
				if (json != null) tileEntities.Add(json);
			}

			return new SerializableChunkData(
				version, 
				pos, 
				chunk.HasHeightmap() ? chunk.GetHeightmap().GetRaw() : null, 
				sectionData, 
				tileEntities, 
				chunk.GetBiomes()
			);
		}

		public static SerializableChunkData Parse(string jsonString, Level level) {
			JObject jsonObject = JObject.Parse(jsonString);
			int sinceVersion = GlobalSaveVersion.GetSinceVersion(jsonObject);
			ChunkPos chunkPos = ChunkPos.CODEC.Decode(jsonObject["pos"] ?? JValue.CreateNull(), sinceVersion).GetOrThrow();

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

			JToken? heightmapToken = jsonObject["heightmap"];
			int[]? heightmap = null;
			if (heightmapToken != null) {
				JArray array = (JArray)heightmapToken;
				heightmap = new int[array.Count];

				for (int i = 0; i < array.Count; i++) {
					DataResult<int> heightResult = Codecs.INT.Decode(array[i], sinceVersion);
					heightmap[i] = heightResult.GetOrThrow();
				}
			}

			RegistryEntry<Biome>[] biomes = new RegistryEntry<Biome>[Level.CHUNK_LENGTH];
			JToken? biomesToken = jsonObject["biomes"];
			if (biomesToken != null) {
				JArray array = (JArray)biomesToken;
				for (int i = 0; i < array.Count; i++) {
					biomes[i] = Biome.ENTRY_CODEC.Decode(array[i], sinceVersion).GetOrThrow();
				}
			}

			return new SerializableChunkData(sinceVersion, chunkPos, heightmap, sectionData, tileEntities, biomes);
		}

		public Chunk Read(Level level, ChunkPos chunkPos) {
			if (!this.chunkPos.Equals(chunkPos)) {
				Logger.LogError("Chunk {} is in the wrong location: expected {}, got {}", chunkPos, chunkPos, this.chunkPos);
			}

			int sectionCount = level.GetSectionCount();
			ChunkSection[] sections = new ChunkSection[sectionCount];
			Func<BlockStateContainer> containerFactory = level.BlockStateContainerFactory();

			foreach (SectionData section in this.sectionData) {
				if (section.chunkSection != null) {
					sections[level.GetSectionIndexFromSectionY(section.y)] = section.chunkSection;
				}
			}

			WorldChunk chunk = new(level, chunkPos, sections, containerFactory);

			foreach (JToken token in this.tileEntities) {
				try {
					BlockPos? blockPos = TileEntity.GetPosFromJson(token);
					if (blockPos is not { } pos) {
						Logger.LogError("Failed to parse TileEntity block pos: {}", token);
						continue;
					}
					ChunkSection section = sections[level.GetSectionIndexFromBlock(pos.y)];
					SectionPos sectionPos = ChunkSection.ComputeLocalPos(pos.x, pos.y);
					BlockState state = section.GetBlockState(sectionPos.x, sectionPos.y);
					TileEntity? tileEntity = TileEntity.FromJson(token, pos, state, this.sinceVersion);
					if (tileEntity != null) {
						chunk.SetTileEntity(tileEntity);
					}
				} catch (Exception e) {
					Logger.LogFatal(e);
				}
			}
			chunk.SyncBlocksWithTileEntities();

			if (this.heightmap != null) {
				Heightmap chunkHeightmap = chunk.GetHeightmap();
				chunkHeightmap.SetRaw(this.heightmap);
			}

			chunk.ReplaceBiomes(this.biomes);

			return chunk;
		}

		public string Write() {
			JObject sections = new();
			foreach (SectionData section in this.sectionData) {
				if (section.chunkSection != null) {
					int sectionY = section.y;
					IEnumerable<int> states = section.chunkSection.GetStatesImmutable().Select(Block.GetRawID);
					JArray array = new(states);
					sections[sectionY.ToString()] = array;
				}
			}

			JArray tileEntities = new();
			foreach (JToken tileEntity in this.tileEntities) {
				tileEntities.Add(tileEntity);
			}

			JArray? heightmapArray = this.heightmap == null ? null : new JArray();
			if (heightmapArray != null) {
				foreach (int height in this.heightmap!) {
					heightmapArray.Add(Codecs.INT.Encode(height));	
				}
			}

			JArray biomes = new();
			foreach (RegistryEntry<Biome> biome in this.biomes) {
				biomes.Add(biome == null ? JValue.CreateNull() : Biome.ENTRY_CODEC.Encode(biome));
			}

			JObject json = new();
			GlobalSaveVersion.WriteVersion(json, this.sinceVersion);
			json["pos"] = ChunkPos.CODEC.Encode(this.chunkPos);
			json["heightmap"] = heightmapArray == null ? JValue.CreateNull() : heightmapArray;
			json["sections"] = sections;
			json["tileEntities"] = tileEntities;
			json["biomes"] = biomes;
			return json.ToString(Formatting.None);
		}

		public record SectionData(int y, ChunkSection? chunkSection);
	}
}
