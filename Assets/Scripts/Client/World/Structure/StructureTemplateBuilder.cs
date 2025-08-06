using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

public class StructureTemplateBuilder {
    public string ID;
    public StructureTemplate.PlacementFunction placementFunction;
    public StructureTemplate.PlacementValidationFunction validationFunction;
    public StructureTemplate.PlacementGenerator placementGenerator;
    public Action<BlockChangeInfo>? blockChangedEvent = null;

    public StructureTemplateBuilder(string ID) => this.ID = ID;

    public static StructureTemplateBuilder CreateNewStructure(string ID) => new StructureTemplateBuilder(ID);

    public StructureTemplateBuilder PlacementFunction(StructureTemplate.PlacementFunction placementFunction) {
        this.placementFunction = placementFunction;
        return this;
    }

    public StructureTemplateBuilder ValidationFunction(StructureTemplate.PlacementValidationFunction validationFunction) {
        this.validationFunction = validationFunction;
        return this;
    }

    public StructureTemplateBuilder PlacementGenerator(StructureTemplate.PlacementGenerator placementGenerator) {
        this.placementGenerator = placementGenerator;
        return this;
    }

    public StructureTemplateBuilder BlockStateChangedCallback(Action<BlockChangeInfo> callback) {
        this.blockChangedEvent = callback;
        return this;
    }

    public StructureTemplate Build() => StructureTemplate.Create(this);
}
