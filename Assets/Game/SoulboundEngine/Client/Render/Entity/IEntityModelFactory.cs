namespace SoulboundEngine.Client.Render.Entity {
	public interface IEntityModelFactory {
		EntityModel GetModel();
	}

	public interface IEntityModelFactory<M> : IEntityModelFactory where M : EntityModel {
		new M GetModel();

		EntityModel IEntityModelFactory.GetModel() => this.GetModel();

		public static IEntityModelFactory<M> OfFactory(EntityModel.Factory<M> factory) {
			return new FactoryImpl(factory);
		}

		protected sealed class FactoryImpl : IEntityModelFactory<M> {
			private readonly EntityModel.Factory<M> factory;

			public FactoryImpl(EntityModel.Factory<M> factory) {
				this.factory = factory;
			}

			public M GetModel() => this.factory();
		}
	}
}
