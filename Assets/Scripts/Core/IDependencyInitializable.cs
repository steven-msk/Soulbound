public interface IDependencyInitializable<TInstance, TDependency> {
	public TInstance OnGameInit(TDependency dependency);
}
