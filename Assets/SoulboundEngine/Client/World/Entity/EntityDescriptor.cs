using SoulboundEngine.Client.World.EntitySystem.Attribute;
using SoulboundEngine.Core.Registry;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem {
	public class EntityDescriptor : IIdentifierProvider {
		private readonly Dictionary<AttributeType, object> definedAttributes = new();
		private readonly Dictionary<AttributeType, IValueRule?> valueRules = new();
		private readonly Identifier identifier;
		private readonly Func<Vector2, Entity> factory;

		public EntityDescriptor(Identifier identifier, Func<Vector2, Entity> factory) {
			this.identifier = identifier;
			this.factory = factory;
		}

		public EntityDescriptor(
				Identifier identifier,
				Func<Vector2, Entity> factory,
				Dictionary<AttributeType, object> definedAttributes,
				Dictionary<AttributeType, IValueRule?> valueRules
			)
			: this(identifier, factory) {
			this.definedAttributes = definedAttributes;
			this.valueRules = valueRules;
		}

		public AttributeContainer CreateAttributeContainer() {
			return new AttributeContainer(definedAttributes, valueRules);
		}

		public Identifier GetIdentifier() => identifier;

		public Entity Create(Vector2 pos) => factory(pos);

		public override string ToString() => identifier.ToString();
	}
}
