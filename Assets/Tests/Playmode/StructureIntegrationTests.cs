using NUnit.Framework;
using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Client.World.Structure;
using SoulboundBackend.Common;
using SoulboundBackend.Core;
using SoulboundBackend.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using WorldTests;

#nullable enable

public class StructureIntegrationTests {
	public static StructureTemplate CreateBoxTemplate(
			int xSize, int ySize, string ID,
			BlockState stateOverride,
			StructureTemplate.PlacementValidationFunction? validationFunction
		) {
		return StructureTemplateBuilder.CreateNewStructure(ID)
			.PlacementFunction((context, forced) =>
				new PreliminaryStructureData(
					new Vector2Int(xSize, ySize),
					new ChunkBlockPos(context.chunkBlockX, 0, context.chunkX),
					forced
				)
			).ValidationFunction(validationFunction ?? ((_, _) => true))
			.PlacementGenerator((context, preliminaryData) => {
				var data = preliminaryData.GetValueOrDefault();
				return new StructurePlacementConstraints(
					new BoundsInt2D(new Vector2Int(context.chunkBlockX, context.chunkBlockY), data.size),
					CreateFilledBoxOverrides(stateOverride, data, context)
				);
			})
			.Build();
	}

	public static Dictionary<ChunkBlockPos, BlockState> CreateFilledBoxOverrides(
			BlockState state,
			PreliminaryStructureData data,
			StructureGenerationContext context
		) {
		Dictionary<ChunkBlockPos, BlockState> stateOverrides = new();
		for (int x = context.chunkBlockX; x < data.size.x + context.chunkBlockX; x++) {
			for (int y = context.chunkBlockY; y < data.size.y + context.chunkBlockY; y++) {
				stateOverrides[new BlockPos(x, y).ToChunkBlockPos(context.chunkX)] = state;
			}
		}
		return stateOverrides;
	}

	[OneTimeSetUp]
	public void OneTimeSetup() {
		SoulboundBackend.Core.Resource.ResourceGroups.Bootstrap();
		StaticResetManager.ResetAll();
	}

	[SetUp, TearDown]
	public void StructureCleanup() {
		Level.ClearStructureRegistry();
	}

	[Test]
	public void StructureAt_ReturnsTrue_WhenStructureIsPlacedAndPersistent() {
		var template = CreateBoxTemplate(5, 5, "testStructure", Blocks.stone.defaultState, null);

		World.CreateAnonymousContext(null, out var worldManager);
		Level level = World.TryGetLevel(worldManager);

		ChunkBlockPos pos = new(0, 0, 0);

		Level.RegisterStructure(template);
		level.ForcePlaceStructure(pos, template);

		BlockPos targetPos = new(1, 1);
		Assert.That(level.StructureAt(targetPos, out var placement));
	}

	[Test]
	public void OverlappingStructures_ReturnsTwo_WhenStructuresPlacedOverlapping() {
		var template1 = CreateBoxTemplate(5, 5, "box1", Blocks.stone.defaultState, null);
		var template2 = CreateBoxTemplate(5, 5, "box2", Blocks.stone.defaultState, null);

		World.CreateAnonymousContext(null, out var worldManager);
		Level level = World.TryGetLevel(worldManager);

		ChunkBlockPos structurePos1 = new(0, 0, 0);
		ChunkBlockPos structurePos2 = new(1, 1, 0);

		Level.RegisterStructure(template1);
		Level.RegisterStructure(template2);

		level.ForcePlaceStructure(structurePos1, template1);
		level.ForcePlaceStructure(structurePos2, template2);

		BlockPos samplePos = new(2, 2);
		Assert.That(level.OverlappingStructures(samplePos, out var overlapping),
			() => "No overlapping structures found");
		Assert.That(overlapping
				.Select(p => p.ID)
				.SequenceEqual(new List<string>() { template1.ID, template2.ID }),
			() => "Overlapping equality failed");
	}

	[Test]
	public void PlacedStructure_OverridesBlockStates() {
		var template = CreateBoxTemplate(10, 10, "box", Blocks.wood.defaultState, null);
		var pos = new ChunkBlockPos(1, -10, 0);

        World.CreateAnonymousContext(null, out var worldManager);
        Level level = World.TryGetLevel(worldManager);

		Level.RegisterStructure(template);
		level.ForcePlaceStructure(pos, template);

		for (int x = 1; x < 9; x++) { 
			for (int y = -0; y < 0; y++) {
				BlockPos targetPos = new(x, y);
				Assert.That(level.StructureAt(targetPos, out var placement),
					() => "No structure found at " + targetPos);
				Assert.That(level.BlockStateAt(targetPos), Is.EqualTo(Blocks.wood.defaultState));
			}
		}
    }
}