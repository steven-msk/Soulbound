using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.World.Generation {
	public sealed class NoiseSampler : INoise {
		private readonly FastNoiseLite noise;
		private readonly Vector3 offset;

		public NoiseSampler(int channel, int seed, NoiseSettings settings) {
			this.noise = new FastNoiseLite(seed);
			noise.SetNoiseType(settings.noiseType);
			noise.SetDomainWarpType(settings.domainWarpType);
			noise.SetFractalType(settings.fractalType);
			noise.SetRotationType3D(settings.rotationType3D);
			noise.SetCellularDistanceFunction(settings.cellularDistanceFunction);
			noise.SetCellularReturnType(settings.cellularReturnType);
			noise.SetFrequency(settings.frequency);
			noise.SetDomainWarpAmp(settings.domainWarpAmp);
			noise.SetFractalGain(settings.fractalGain);
			noise.SetFractalLacunarity(settings.fractalLacunarity);
			noise.SetFractalOctaves(settings.fractalOctaves);
			noise.SetFractalPingPongStrength(settings.fractalPingPingStrength);
			noise.SetFractalWeightedStrength(settings.fractalWeightedStrength);
			noise.SetCellularJitter(settings.cellularJitter);

			float offsetX = OffsetAxis(seed, channel, 0);
			float offsetY = OffsetAxis(seed, channel, 1);
			float offsetZ = OffsetAxis(seed, channel, 2);
			this.offset = new Vector3(offsetX, offsetY, offsetZ);
		}

		private int OffsetAxis(int seed, int channel, int axis) {
			return (int)(OffsetChannel(seed, channel * 2 + axis) % 200000u) - 100000;
		}

		static int OffsetChannel(int seed, int channel) {
			unchecked {
				uint n = (uint)(seed ^ (channel * 0x9E3779B9));
				n ^= n >> 21;
				n *= 0x85EBCA6B;
				n ^= n >> 13;
				n *= 0xC2B2AE35;
				n ^= n >> 21;
				return (int)n;
			}
		}

		public float Sample1D(float x) {
			return noise.GetNoise(x + offset.x, 0f);
		}

		public float Sample2D(float x, float y) {
			return noise.GetNoise(x + offset.x, y + offset.y);
		}

		public float Sample3D(float x, float y, float z) {
			return noise.GetNoise(x + offset.x, y + offset.y, z + offset.z);
		}
	}
}
