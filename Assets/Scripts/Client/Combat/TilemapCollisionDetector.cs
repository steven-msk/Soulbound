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

		public void NotifyHit(Hitbox hitbox, Collider2D collider) {
			Vector2 estimatedPos = hitbox.GetCollider().ClosestPoint(collider.transform.position);
			GameObject hitEffect = GameObject.Instantiate(blockHitEffectPrefab, estimatedPos, Quaternion.identity);

			BlockPos blockPos = (BlockPos)estimatedPos;
			Block blockHit = Soulbound.instance.GetActiveLevel().BlockAt(blockPos);
			hitEffect.GetComponent<BlockHitEffect>().Initialize(blockHit, blockPos, this.GetComponent<Tilemap>());
		}
	}
}
