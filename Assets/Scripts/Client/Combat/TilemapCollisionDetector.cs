using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SoulboundBackend.Client.Combat {
	[RequireComponent(typeof(Tilemap))]
	public sealed class TilemapCollisionDetector : MonoBehaviour {
		[SerializeField] private GameObject blockHitEffectPrefab;
		private AttackContext previousContext;

		public void NotifyHit(AttackContext context, Hitbox hitbox, Collider2D collider) {
			if (previousContext == context) {
				return;
			}
			this.previousContext = context;
			var colliderDistance = Physics2D.Distance(hitbox.GetCollider(), collider);
			Vector2 estimatedPos = colliderDistance.pointB;
			
			GameObject hitEffect = GameObject.Instantiate(blockHitEffectPrefab, estimatedPos, Quaternion.identity);
			hitEffect.GetComponent<BlockHitEffect>().Initialize(this.GetComponent<Tilemap>());
		}
	}
}
