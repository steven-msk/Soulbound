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

		public void Initialize(ITilemap tilemap) {
			var renderer = particles.GetComponent<ParticleSystemRenderer>();

			Texture2D text = new(1, 1);
			ColorUtility.TryParseHtmlString("#453323", out var brown);
			text.SetPixel(0, 0, brown);
			text.Apply(true);

			renderer.material = new Material(Shader.Find("Sprites/Default"));
			renderer.material.mainTexture = text;

			particles.Play();
			Destroy(gameObject, particles.main.duration + 0.1f);
		}
	}
}
