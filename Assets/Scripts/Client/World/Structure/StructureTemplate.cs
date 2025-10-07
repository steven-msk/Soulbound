using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Common;
using System;
using System.Collections.Generic;

namespace SoulboundBackend.Client.World.Structure {
	public class StructureTemplate {
		/// <summary>
		/// Function which returns true if the structure should be placed in the given context.
		/// </summary>
		/// <param name="context">
		/// The context used to decide whether to sketch out the structure at some certain position.
		/// </param>
		/// <returns>The rough data of a placement. The return value can be null.</returns>
		public delegate PreliminaryStructureData? PlacementFunction(
			StructureGenerationContext context, 
			bool forcePlacement
		);

		/// <summary>
		/// Function that validates the given preliminary data for the structure.
		/// </summary>
		/// <param name="context">The context which the structure is built in.</param>
		/// <param name="preliminaryData">
		/// The preliminary structure data generated from the placement function.
		/// </param>
		/// <returns>True or false depending on whether the data is valid for placement.</returns>
		public delegate bool PlacementValidationFunction(
			StructureGenerationContext context,
			PreliminaryStructureData preliminaryData
		);

        // TODO: proper spawn conditions for structure templates

        /// <summary>
        /// Function which generates the placement itself.
        /// </summary>
        /// <param name="context">The context of the placement.</param>
        /// <param name="preliminaryData">The preliminary data of the structure.</param>
        /// <returns>
        /// The constraints of the structure. Pass this in <see
        /// cref="FinalizePlacement(StructurePlacementConstraints)"/> to generate the final placement.
        /// </returns>
        public delegate StructurePlacementConstraints PlacementGenerator(
            StructureGenerationContext context,
            PreliminaryStructureData? preliminaryData
        );

        public string ID { get; private set; }
		public PlacementFunction placementFunction { get; private set; }
		public PlacementValidationFunction validationFunction { get; private set; }
		public PlacementGenerator placementGenerator { get; private set; }
		public Action<BlockChangeInfo> blockStateChangedCallback { get; private set; }

		private StructureTemplate(
				string ID,
				PlacementFunction placementFunction,
				PlacementValidationFunction validationFunction,
				PlacementGenerator placementGenerator,
				Action<BlockChangeInfo> blockStateChangedCallback
			) {
			this.ID = ID;
			this.placementFunction = placementFunction;
			this.validationFunction = validationFunction;
			this.placementGenerator = placementGenerator;
			this.blockStateChangedCallback = blockStateChangedCallback;
		}

		public static StructureTemplate Create(StructureTemplateBuilder builder) {
			return new StructureTemplate(builder.ID, builder.placementFunction, builder.validationFunction,
				builder.placementGenerator, builder.blockChangedEvent);
		}

		public StructurePlacement FinalizePlacement(StructurePlacementConstraints placementConstraints) {
			int xMin = placementConstraints.bounds.xMin;
			int yMin = placementConstraints.bounds.yMin;
            ChunkBlockPos origin = ChunkBlockPos.FromBlockPos(new BlockPos(xMin, yMin));
			Dictionary<ChunkBlockPos, BlockState> stateOverrides = placementConstraints.stateOverrides;
			BoundsInt2D bounds = placementConstraints.bounds;
			return new StructurePlacement(origin, this.ID, stateOverrides, bounds);
		}
	}
}