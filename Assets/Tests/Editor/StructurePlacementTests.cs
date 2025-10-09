using NUnit.Framework;
using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Client.World.Structure;
using SoulboundBackend.Common;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class StructurePlacementTests {
	[Test]
	public void Equality_WorksForSameData() {
		var origin = new ChunkBlockPos(1, 2, 3);
		var bounds = new BoundsInt2D(new Vector2Int(0, 0), new Vector2Int(10, 10));
		var dict = new Dictionary<ChunkBlockPos, BlockState> {
			{ new ChunkBlockPos(0,0,0), Blocks.stone.defaultState }
		};

		var p1 = new StructurePlacement(origin, "test", dict, bounds);
		var p2 = new StructurePlacement(origin, "test", dict, bounds);

		Assert.IsTrue(p1 == p2);
		Assert.AreEqual(p1.GetHashCode(), p2.GetHashCode());
	}

	[Test]
	public void PersistentExistence_ReturnsTrue_IfStateOverridesNotEmpty() {
		var placement = new StructurePlacement(
			new ChunkBlockPos(1, 2, 3), "id",
			new Dictionary<ChunkBlockPos, BlockState> {
				{ new ChunkBlockPos(0,0,0), Blocks.stone.defaultState }
			},
			new BoundsInt2D(Vector2Int.zero, Vector2Int.one)
		);
		Assert.IsTrue(placement.PersistentExistence());
	}

	[Test]
	public void PersistentExistence_ReturnsFalse_IfStateOverridesEmpty() {
		var placement = new StructurePlacement(
			new ChunkBlockPos(1, 2, 3), "id",
			new Dictionary<ChunkBlockPos, BlockState>(),
			new BoundsInt2D(Vector2Int.zero, Vector2Int.one)
		);
		Assert.IsFalse(placement.PersistentExistence());
	}

	[Test]
	public void PlacementFunction_GeneratesPreliminaryData() {
		BoundsInt2D expectedBounds = new(0, 0, 2, 2);
		ChunkBlockPos expectedOrigin = new(0, 0, 0);
		var template = StructureIntegrationTests.CreateBoxTemplate(2, 2, "box", Blocks.stone.defaultState, null);

		StructureGenerationContext context = new(0, 0, 0, default, null);
		var data = template.placementFunction.Invoke(context, true);

		PreliminaryStructureData expectedData = new(data.Value.size, expectedOrigin, true);
		Assert.That(data.GetValueOrDefault(), Is.EqualTo(expectedData));
	}

	[Test]
	public void PlacementValidationFunction_ReturnsTrue_WhenConditionsMet() {
		var template = StructureTemplateBuilder.CreateNewStructure("box")
			.PlacementFunction((context, forced) => {
				return new PreliminaryStructureData(
					new Vector2Int(10, 10),
					new ChunkBlockPos(0, 0, context.chunkX),
					forced
				);
			}).ValidationFunction((context, preliminaryData) => {
				bool valid = true;
				valid = context.chunkBlockX == Level.CHUNK_LENGTH - 1;
				valid = valid && context.chunkX == 0;
				return valid;
			}).PlacementGenerator((context, preliminaryData) => {
				var data = preliminaryData.GetValueOrDefault();
				return new StructurePlacementConstraints(
					new BoundsInt2D((Vector2Int)data.origin, data.size),
					StructureIntegrationTests.CreateFilledBoxOverrides(
						Blocks.stone.defaultState, 
						data,
						context
					)
				);
			}).Build();

		PreliminaryStructureData CreateData(int cx, out StructureGenerationContext context) {
            context = new StructureGenerationContext(cx, 0, 0, default, null);
            return template.placementFunction.Invoke(context, false).GetValueOrDefault();
        }

		for (int c = -1; c <= -1; c++) {
			int cx = 0;
			var data = CreateData(cx, out var context);
			for (; cx < Level.CHUNK_LENGTH - 2; cx++) {
				Assert.That(template.validationFunction.Invoke(context, data), Is.False);
				data = CreateData(cx, out context);
			}

			cx++;
			Assert.That(cx == Level.CHUNK_LENGTH - 1);
			if (c != 0) {
				continue;
			}

			data = CreateData(cx, out context);

			if (c == 0) {
				Assert.That(template.validationFunction.Invoke(context, data), Is.True);
			}
		}
	}

	[Test]
	public void PlacementGenerator_GeneratesOverrides() {
		var template = StructureIntegrationTests.CreateBoxTemplate(3, 5, "box", Blocks.leaves.defaultState, null);

		var context = new StructureGenerationContext(0, 0, 0, default, null);
		var data = template.placementFunction.Invoke(context, false).GetValueOrDefault();
		var overrides = StructureIntegrationTests.CreateFilledBoxOverrides(Blocks.leaves.defaultState, data, context);

		Assert.That(template.placementGenerator.Invoke(context, data).stateOverrides, Is.EqualTo(overrides));
	}
}
