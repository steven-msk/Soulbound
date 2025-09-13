using System;

namespace SoulboundBackend.Client.ItemSystem.Attack {
	public class AttackProcedureNotFoundException : NullReferenceException {
		public AttackProcedureNotFoundException(string weapon, ItemUseTrigger trigger)
			: base($"Weapon attack procedure not found: input: '{trigger}', weapon: '{weapon}'") { }
	}
}
