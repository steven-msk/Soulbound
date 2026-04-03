namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public interface IAttributeContext {
		bool TryGetAttribute<T>(Entity entity, AttributeType<T> type, out T value);
	}
}
