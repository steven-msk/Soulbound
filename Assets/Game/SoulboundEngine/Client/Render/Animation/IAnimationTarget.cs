namespace SoulboundEngine.Client.Render.Animation {
	public interface IAnimationTarget<T> {
		T Get();
		void Set(T value);
	}
}
