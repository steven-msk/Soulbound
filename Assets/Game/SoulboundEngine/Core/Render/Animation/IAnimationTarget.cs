namespace SoulboundEngine.Core.Render.Animation {
	public interface IAnimationTarget<T> {
		T Get();
		void Set(T value);
	}
}
