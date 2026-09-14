namespace SoulboundEngine.UnityClient.Editor.Tools.WorldGen {
	using SoulboundEngine.Registry;
	using SoulboundEngine.UnityClient.Assets;
	using SoulboundEngine.UnityClient.Debug.Logging;
	using SoulboundEngine.UnityClient.Render.Block;
	using SoulboundEngine.World;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Chunk;
	using SoulboundEngine.World.Gen;
	using SoulboundEngine.World.Gen.Biome;
	using SoulboundEngine.World.Gen.Noise;
	using SoulboundEngine.World.Level;
	using System.Linq;
	using UnityEditor;
	using UnityEditor.SceneManagement;
	using UnityEngine;
	using UnityEngine.Tilemaps;

	public class EditorWorldGenTool {
		private readonly EditorWorldGenContext context;

		public EditorWorldGenTool(EditorWorldGenContext context) {
			this.context = context;
		}


		[MenuItem("Soulbound/World gen tools/Switch to WorldGenScene")]
		static void SwitchToTool() {
			SoulboundEngine.Logger.SetWrapper(new UnityClientLoggerWrapper(UnityEngine.Debug.unityLogger));
			EditorSceneManager.OpenScene("Assets/Game/Content/Editor/Tools/WorldGenScene.unity");
			AssetManager.LoadAllWithPreloadLabel();
			new EditorWorldGenTool(new EditorWorldGenContext()).Init();
		}

		public void Init() {
			this.context.Init();
			BlockRenderManager blockRenderManager = new(Registries.BLOCK.ToList());

			NoiseLevelChunkGenerator chunkGenerator = this.context.CreateChunkGenerator(NoiseGeneratorSettings.DEFAULT, MultiNoiseBiomeSourceParamList.DEFAULT);
			Chunk chunk = new EditorWorldGenChunk(ChunkPos.ORIGIN, IHeightLimitView.Create(Level.DEFAULT_MIN_Y, Level.DEFAULT_WORLD_HEIGHT), () => new BlockStateContainer(ChunkSection.WIDTH, ChunkSection.HEIGHT));

			RandomState randomState = this.context.CreateRandomState(NoiseGeneratorSettings.DEFAULT, 0L);
			chunk = chunkGenerator.MapBiomes(randomState, chunk);
			chunk = chunkGenerator.Fill(randomState, chunk);
			chunk = chunkGenerator.BuildSurface(randomState, chunk);

			BlockPos.Mutable blockPos = new();
			Tilemap tilemap = Object.FindFirstObjectByType<Tilemap>();
			for (int x = 0; x < Level.CHUNK_LENGTH; x++) {
				for (int y = Level.DEFAULT_MIN_Y; y <= Level.DEFAULT_MAX_Y; y++) {
					blockRenderManager.Render(tilemap, x, y, chunk.GetBlockState(blockPos.Set(x, y)));
				}
			}
		}
	}
}
