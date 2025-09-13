namespace SoulboundBackend.Client.ItemSystem {
	public interface IAttackPerformer : IItemCapability {
		void PerformAttack(ItemUseTrigger useTrigger);
	}
}

