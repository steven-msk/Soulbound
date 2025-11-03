using UnityEngine;

namespace SoulboundBackend.Client.Combat {
	public class HitFlashEffect : MonoBehaviour {
		[SerializeField] private SpriteRenderer spriteRenderer;
		private Material materialInstance;
		private float flashStep;

		private void Awake() {
			materialInstance = Instantiate(spriteRenderer.material);
			spriteRenderer.material = materialInstance;
		}

		public void TriggerFlash() => flashStep = 1f;

		private void Update() {
			if (flashStep > 0) {
				flashStep -= Time.deltaTime * 5f;
				materialInstance.SetFloat("_FlashStep", flashStep);
			}
		}
	}
}
