using System;

public class AttackProcedureNotFoundException : NullReferenceException {
	public AttackProcedureNotFoundException(string weapon, ItemUseTrigger trigger)
		: base($"Weapon attack procedure not found: input: '{trigger}', weapon: '{weapon}'") { }
}