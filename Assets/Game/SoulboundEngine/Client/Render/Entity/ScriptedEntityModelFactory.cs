using SoulboundEngine.Client.World.EntitySystem;
using SoulboundEngine.Core.Registry;
using System;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.Render.Entity {
	public sealed class ScriptedEntityModelFactory<M> : IEntityModelFactory<M> where M : EntityModel {
		public readonly Identifier identifier;
		private readonly Func<GameObject, M> modelSupplier;
		private readonly Func<M>? fallback;

		public ScriptedEntityModelFactory(EntityDescriptor descriptor, Func<GameObject, M> modelSupplier, Func<M>? fallback = null)
			: this(EntityDescriptor.GetIdentifier(descriptor), modelSupplier, fallback) {
		}

		public ScriptedEntityModelFactory(Identifier identifier, Func<GameObject, M> modelSupplier, Func<M>? fallback = null) {
			this.identifier = identifier;
			this.modelSupplier = modelSupplier;
			this.fallback = fallback;
		}

		public M GetModel(ScriptedEntityModelManager scriptedEntityModelManager) {
			ScriptedEntityModel model = scriptedEntityModelManager.Get(this.identifier);
			GameObject obj = model.GetGameObject();

			if (obj == null && this.fallback != null) return this.fallback();
			else Debug.Logging.Logger.LogError("Could not find scripted entity model for '{}'", this.identifier);

			return this.modelSupplier(obj!);
		}
	}
}
