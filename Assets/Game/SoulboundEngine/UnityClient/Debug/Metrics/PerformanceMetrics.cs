namespace SoulboundEngine.UnityClient.Debug.Metrics {
	using System;
	using UnityEngine;
	using UnityEngine.Profiling;

	public sealed class PerformanceMetrics {
		private long lastFrameMemory;
		public float instantFps { get; private set; }
		public float frameTime { get; private set; }
		public int gcAllocBytesThisFrame { get; private set; }
		public float TotalManagedMemoryMB => GC.GetTotalMemory(false) / 1024f / 1024f;
		public float TotalUnityReservedMemoryMB => Profiler.GetTotalReservedMemoryLong() / 1024f / 1024f;
		public float MonoHeapMB => Profiler.GetMonoHeapSizeLong() / 1024f / 1024f;
		public float MonoUsedMB => Profiler.GetMonoUsedSizeLong() / 1024f / 1024f;
		public float GPUManagedMemoryMB => Profiler.GetAllocatedMemoryForGraphicsDriver() / 1024f / 1024f;
		public float GPUReservedMemoryMB => SystemInfo.graphicsMemorySize;

		public void Update() {
			this.instantFps = 1f / Time.unscaledDeltaTime;
			this.frameTime = Time.unscaledDeltaTime * 1000f;

			long currentMemory = GC.GetTotalMemory(false);
			this.gcAllocBytesThisFrame = (int)(currentMemory - this.lastFrameMemory);
			this.lastFrameMemory = currentMemory;
		}
	}
}
