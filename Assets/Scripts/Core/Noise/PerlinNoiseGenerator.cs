using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class PerlinNoiseGenerator : INoiseGenerator {
	public const int OFFSET_RANGE = 100_000;
	private float scaleX, scaleY;
	private float offsetX, offsetY;

	public PerlinNoiseGenerator(int seed, Vector2 scale) : this(seed, scale.x, scale.y) { }

	public PerlinNoiseGenerator(int seed, float scale) : this(seed, scale, scale) { }

	public PerlinNoiseGenerator(int seed, float scaleX, float scaleY) {
		this.scaleX = scaleX;
		this.scaleY = scaleY;
		System.Random rng = new System.Random(seed);
		offsetX = rng.Next(-OFFSET_RANGE, OFFSET_RANGE);
		offsetY = rng.Next(-OFFSET_RANGE, OFFSET_RANGE);
	}

	public float GenerateNoise(float x, float y) => Mathf.PerlinNoise(x * scaleX + offsetX, y * scaleY + offsetY);
}
