using System;

namespace SoulboundEngine.Client.World.EntitySystem.Transform {
	public interface ITransformSupplier<E> where E : Entity {
		IEntityTransform GetTransform(E entity);

		public static ITransformSupplier<E> Of(Func<E, IEntityTransform> supplier) {
			return new Delegated(supplier);
		}

		private sealed class Delegated : ITransformSupplier<E> {
			private readonly Func<E, IEntityTransform> supplier;

			public Delegated(Func<E, IEntityTransform> supplier) {
				this.supplier = supplier;
			}

			public IEntityTransform GetTransform(E entity) {
				return supplier(entity);
			}
		}
	}
}
