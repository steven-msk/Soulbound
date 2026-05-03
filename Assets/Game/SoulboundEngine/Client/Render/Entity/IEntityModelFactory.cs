namespace SoulboundEngine.Client.Render.Entity {
	public interface IEntityModelFactory {
		EntityModel GetModel(Context context);

		public sealed record Context(ScriptedEntityModelManager scriptedEntityModelManager);
	}

	public interface IEntityModelFactory<M> : IEntityModelFactory where M : EntityModel {
		new M GetModel(Context context);

		EntityModel IEntityModelFactory.GetModel(Context context) {
			return this.GetModel(context);
		}

		public static IEntityModelFactory<M> OfFactory(EntityModel.Factory<M> factory) {
			return new FactoryImpl(factory);
		}

		protected sealed class FactoryImpl : IEntityModelFactory<M> {
			private readonly EntityModel.Factory<M> factory;

			public FactoryImpl(EntityModel.Factory<M> factory) {
				this.factory = factory;
			}

			public M GetModel(Context context) => this.factory();
		}
	}
}
