using UnityEngine;

public class PerlinNoiseGenerator1D : INoiseGenerator1D {
	private float scale;
	private float offset;

	public PerlinNoiseGenerator1D(int seed, float scale) {
		this.scale = scale;
		System.Random rng = new System.Random(seed);
		offset = rng.Next(-PerlinNoiseGenerator.OFFSET_RANGE, PerlinNoiseGenerator.OFFSET_RANGE);
	}

	public float GenerateNoise1D(float x) => Mathf.PerlinNoise1D(x * scale + offset);
}
