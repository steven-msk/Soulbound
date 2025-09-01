using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AttackProcedureNotFoundException : NullReferenceException {
	public AttackProcedureNotFoundException(string weapon, ItemUseTrigger trigger)
		: base($"Weapon attack procedure not found: input: '{trigger}', weapon: '{weapon}'") { }
}