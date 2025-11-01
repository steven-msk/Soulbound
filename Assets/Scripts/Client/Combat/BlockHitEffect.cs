using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.BlockSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SoulboundBackend.Client.Combat {
	public sealed class BlockHitEffect : MonoBehaviour {
		[SerializeField] private ParticleSystem particles;

		public void Initialize(Block blockHit, BlockPos blockPos, ITilemap tilemap) {
			var renderer = particles.GetComponent<ParticleSystemRenderer>();
			var tileData = new TileData();
			blockHit.tileReference?.GetTileData((Vector3Int)blockPos, tilemap, ref tileData);

			renderer.material = new Material(Shader.Find("Sprites/Default"));
			renderer.material.mainTexture = tileData.sprite.texture;

			particles.Play();
			Destroy(gameObject, particles.main.duration + 0.1f);
		}
	}
}
