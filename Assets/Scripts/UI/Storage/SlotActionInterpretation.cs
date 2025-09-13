using static InventoryController;

public record SlotActionInterpretation {
	public string context;
	public int priority;
	public InterpretationFunction interpretationFunction;

	public SlotActionInterpretation(string context, int priority, InterpretationFunction interpretationFunction) {
		this.context = context;
		this.priority = priority;
		this.interpretationFunction = interpretationFunction;
	}
}