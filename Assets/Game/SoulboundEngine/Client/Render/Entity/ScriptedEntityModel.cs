using SoulboundEngine.Core.Registry;
using UnityEngine;

namespace SoulboundEngine.Client.Render.Entity {
	[CreateAssetMenu(menuName = "Entity Model/Scripted Entity Model")]
	public sealed class ScriptedEntityModel : ScriptableObject {
		[Tooltip("Must match an Identifier declared in code and follow parsing rules (e.g. soulbound:entity)")]
		[SerializeField]
		private string identifier;
		[Tooltip("The game object instantiated at runtime")]
		[SerializeField]
		private GameObject gameObject;

		public Identifier GetIdentifier() => Identifier.Of(this.identifier);
		public GameObject GetGameObject() => this.gameObject;
	}
}
