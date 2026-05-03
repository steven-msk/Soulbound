namespace SoulboundEngine.Client.Render.Entity {
	public interface IEntityModelFactory {
		EntityModel GetModel(ScriptedEntityModelManager scriptedEntityModelManager);
	}

	public interface IEntityModelFactory<M> : IEntityModelFactory where M : EntityModel {
		new M GetModel(ScriptedEntityModelManager scriptedEntityModelManager);

		EntityModel IEntityModelFactory.GetModel(ScriptedEntityModelManager scriptedEntityModelManager) {
			return this.GetModel(scriptedEntityModelManager);
		}

		public static IEntityModelFactory<M> OfFactory(EntityModel.Factory<M> factory) {
			return new FactoryImpl(factory);
		}

		protected sealed class FactoryImpl : IEntityModelFactory<M> {
			private readonly EntityModel.Factory<M> factory;

			public FactoryImpl(EntityModel.Factory<M> factory) {
				this.factory = factory;
			}

			public M GetModel(ScriptedEntityModelManager scriptedEntityModelManager) => this.factory();
		}
	}
}
