namespace SoulboundEngine.Client.Render.Entity {
	public abstract class EntityModel {
		public delegate M Factory<M>() where M : EntityModel;
	}
}
