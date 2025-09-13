using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class TreeStructure {
    public const int minHeight = 5;
    public const int maxHeight = 20;
    public const int crownRadius = 2;
    public const float treeFrequency = 0.5f;
    public const float treeDisparity = 0.5f;

    public static readonly StructureTemplate instance = StructureTemplateBuilder.CreateNewStructure("tree")
        .PlacementFunction((generationContext, forcePlacement) => {
            float noise = Mathf.PerlinNoise(generationContext.chunkBlockX * treeFrequency, generationContext.seed);
            if (noise <= treeDisparity && !forcePlacement) {
                return null;
            }
            int worldX = generationContext.level.ToWorldX(generationContext.chunkBlockX, generationContext.chunkX);
            int ypos = generationContext.heightmapData.surfaceLevels[worldX];
            ChunkBlockPos origin = new ChunkBlockPos(generationContext.chunkBlockX, ypos, generationContext.chunkX);

            int height = Random.Range(minHeight, maxHeight + 1);
            BoundsInt2D bounds = new(origin.x - crownRadius, origin.y, crownRadius * 2 + 1, height);
            return new PreliminaryStructureData(bounds, origin, forcePlacement);
        })
        .ValidationFunction((generationContext, preliminaryData) => {
            if (preliminaryData.forced) {
                return true;
            }
            bool valid = true;
            int worldX = generationContext.level.ToWorldX(preliminaryData.origin.x, generationContext.chunkX);
            int ypos = generationContext.heightmapData.surfaceLevels[worldX];
            valid = preliminaryData.origin.y == generationContext.heightmapData.surfaceLevels[worldX];
            int sizeY = preliminaryData.estimatedBounds.size.y;
            valid = sizeY >= minHeight && sizeY <= maxHeight;
            return valid;
        })
        .PlacementGenerator((generationContext, preliminaryData) => {
            Dictionary<ChunkBlockPos, BlockState> stateOverrides = new();
            Level level = generationContext.level;
            ChunkBlockPos origin = preliminaryData.Value.origin;
            BoundsInt2D bounds = preliminaryData.Value.estimatedBounds;
            ChunkBlockPos trunkPos = new ChunkBlockPos(origin.x, origin.y, generationContext.chunkX);
            BlockState woodBlock = Blocks.wood.defaultState;
            BlockState leavesBlock = Blocks.leaves.defaultState;

            for (int ty = 0; ty < bounds.size.y; ty++) {
                stateOverrides[trunkPos] = woodBlock;
                trunkPos.y++;
            }
            Dictionary<int, List<int>> rowToXs = new();
            float angularStep = 1f;
            for (float angle = 0; angle < 360f; angle += angularStep) {
                float rad = angle * Mathf.Deg2Rad;
                int x = Mathf.RoundToInt(trunkPos.x + crownRadius * Mathf.Cos(rad));
                int y = Mathf.RoundToInt(trunkPos.y + crownRadius * Mathf.Sin(rad));
                if (!rowToXs.ContainsKey(y)) {
                    rowToXs[y] = new List<int>();
                }
                rowToXs[y].Add(x);
            }
            foreach (var kvp in rowToXs) {
                int y = kvp.Key;
                List<int> xs = kvp.Value;
                for (int x = xs.Min(); x <= xs.Max(); x++) {
                    BlockPos blockPos = new(level.ToWorldX(x, generationContext.chunkX), y);
                    ChunkBlockPos leafChunkPos = blockPos.ToChunkBlockPos(level.ChunkXAt(blockPos.x));
                    stateOverrides[leafChunkPos] = leavesBlock;
                }
            }
            return new StructurePlacementConstraints(origin, bounds, stateOverrides);
        })
        .BlockStateChangedCallback(blockChangedEvent => {
            Level level = blockChangedEvent.level;
            bool foundStructure = level.StructureAt(blockChangedEvent.pos, out var structure);
            BlockPos changePos = blockChangedEvent.pos;
            BlockState oldState = blockChangedEvent.oldState;
            bool flag_brokenUnderneath = false;
            if (!foundStructure) {
                BlockPos upNeighbor = changePos.GetAdjacent(Direction.Up);
                foundStructure = level.StructureAt(upNeighbor, out structure);
                if (!foundStructure || ChunkBlockPos.FromBlockPos(upNeighbor) != structure.origin) {
                    return;
                }
                flag_brokenUnderneath = true;
            }
            if (oldState.block == Blocks.leaves && structure.stateOverrides.ContainsKey(ChunkBlockPos.FromBlockPos(changePos))) {
                if (level.OverlappingStructures(changePos, out var overlapping)) {
                    foreach (StructurePlacement overlappingStructure in overlapping) {
                        if (overlappingStructure == structure && overlappingStructure.bounds.Contains((Vector2Int)changePos)) {
                            level.MarkStructureDirty(structure);
                            break;
                        }
                    }
                }
                return;
            }
            if ((oldState.block == Blocks.wood && structure.stateOverrides.ContainsKey(ChunkBlockPos.FromBlockPos(changePos))) || flag_brokenUnderneath) {
                level.MarkStructureDirty(structure);
                var toRemove = structure.stateOverrides.Where(stateOverride => stateOverride.Key.y > changePos.y);
                toRemove.ToList().ForEach(stateOverride => {
                    structure.stateOverrides.Remove(stateOverride.Key);
                    level.BreakBlock(stateOverride.Key.ToWorldBlockPos(), BreakSource.NonPlayer);
                });
            }
        }).Build();
}