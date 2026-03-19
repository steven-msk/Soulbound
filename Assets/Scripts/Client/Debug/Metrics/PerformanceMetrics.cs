using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Profiling;

namespace SoulboundBackend.Client.Debug.Metrics {
	public sealed class PerformanceMetrics {
		private long lastFrameMemory;
		public float InstantFps { get; private set; }
		public float FrameTime { get; private set; }
		public float FixedUpdateTime { get; private set; }
		public int GcAllocBytesThisFrame { get; private set; }
		public float TotalManagedMemoryMB => GC.GetTotalMemory(false) / 1024f / 1024f;
		public float TotalUnityReservedMemoryMB => Profiler.GetTotalReservedMemoryLong() / 1024f / 1024f;
		public float MonoHeapMB => Profiler.GetMonoHeapSizeLong() / 1024f / 1024f;
		public float MonoUsedMB => Profiler.GetMonoUsedSizeLong() / 1024f / 1024f;
		public float GPUManagedMemoryMB => Profiler.GetAllocatedMemoryForGraphicsDriver() / 1024f / 1024f;
		public float GPUReservedMemoryMB => SystemInfo.graphicsMemorySize;

		public void Tick() {
			InstantFps = 1f / Time.unscaledDeltaTime;
			FrameTime = Time.unscaledDeltaTime * 1000f;
			FixedUpdateTime = Time.fixedUnscaledDeltaTime * 1000f;

			long currentMemory = GC.GetTotalMemory(false);
			GcAllocBytesThisFrame = (int)(currentMemory - lastFrameMemory);
			lastFrameMemory = currentMemory;
		}
	}
}
