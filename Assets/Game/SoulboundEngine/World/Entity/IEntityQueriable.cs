namespace SoulboundEngine.World.Entity {
	using SoulboundEngine.Common;
	using System;

#nullable enable

	public interface IEntityQueriable<T> {
		T? GetEntity(Guid guid);
	}

	public static class EntityQueriableDefaults {
		public static bool TryGetEntity<T>(this IEntityQueriable<T> queriable, Guid guid, out T entity) {
			entity = queriable.GetEntity(guid)!;
			return entity != null;
		}

		public static Optional<T> GetOptionalEntity<T>(this IEntityQueriable<T> queriable, Guid guid) {
			return Optional<T>.Of(queriable.GetEntity(guid));
		}
	}
}
