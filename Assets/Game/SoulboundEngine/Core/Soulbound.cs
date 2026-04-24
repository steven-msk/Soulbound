using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SoulboundEngine.Client;
using SoulboundEngine.Client.Debug.Metrics;
using SoulboundEngine.Common.Json;
using SoulboundEngine.Core.Assets;
using SoulboundEngine.Core.GameStates;
using SoulboundEngine.Core.Registry;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace SoulboundEngine.Core {
	public sealed class Soulbound : IApplicationController, IDebugMetricsSource {
		private static Soulbound instance;
		public static readonly JsonSerializerSettings globalJsonSettings = new() {
			TypeNameHandling = TypeNameHandling.Auto,
			TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
			Converters = new List<JsonConverter> {
				new Vector2JsonConverter(),
				new Vector3JsonConverter(),
				new ColorJsonConverter()
			},
		};
		private bool running;
		private readonly SoulboundClient client;
		private readonly GameConfig config;
		private readonly PerformanceMetrics performanceMetrics;
		private readonly DebugMetricsService debugMetricsService;

		public Soulbound(GameConfig config) {
			instance = this;
			this.config = config;
			GameStateManager.SetBootstrapping();

			debugMetricsService = new DebugMetricsService();
			performanceMetrics = new PerformanceMetrics();
			RegisterDebugMetricsSource(this);

			AssetManager.PreloadAll();

			client = new SoulboundClient(config, new ClientInit {
				debugMetricsService = this.debugMetricsService
			});

			Registries.Init();
			Registries.Freeze();

			GameStateManager.SetInitialized();
		}

		public void Launch() {
			if (running) return;
			GameStateManager.SetLaunching();

			try {
				Thread.CurrentThread.Name = "LaunchThread";
			} catch (InvalidOperationException) {
			}

			Application.quitting += ((IApplicationController)this).OnApplicationQuit;

			UniTask.Post(async () => {
				client.Start();

				while (running) {
					await UniTask.NextFrame();
					Update();
				}
			});

			running = true;
			GameStateManager.SetRunning();
		}

		public void Update() {
			performanceMetrics.Tick();
			client.Update();
		}

		void IApplicationController.OnApplicationQuit() {
			GameStateManager.SetShutdown();

			client.Shutdown();
			AssetManager.Shutdown();

			GameStateManager.SetTerminated();
		}

		public void RegisterDebugMetricsSource(IDebugMetricsSource source) {
			debugMetricsService.RegisterSource(source);
		}
		public void UnregisterDebugMetricsSource(IDebugMetricsSource source) {
			debugMetricsService.UnregisterSource(source);
		}

		void IDebugMetricsSource.CollectDebugData(ref DebugMetricsBuilder builder) {
			builder.Add("fps", performanceMetrics.InstantFps);
			builder.Add("frameTime", performanceMetrics.FrameTime);
			builder.Add("fixedUpdateTime", performanceMetrics.FixedUpdateTime);
			builder.Add("totalManagedMemory", performanceMetrics.TotalManagedMemoryMB);
			builder.Add("totalUnityReservedMemory", performanceMetrics.TotalUnityReservedMemoryMB);
			builder.Add("monoHeap", performanceMetrics.MonoHeapMB);
			builder.Add("monoUsed", performanceMetrics.MonoUsedMB);
			builder.Add("gpuManagedMemory", performanceMetrics.GPUManagedMemoryMB);
			builder.Add("gpuReservedMemory", performanceMetrics.GPUReservedMemoryMB);
			builder.Add("gcAlloc", performanceMetrics.GcAllocBytesThisFrame);
		}

		public PerformanceMetrics GetPerformanceMetrics() => performanceMetrics;

		public static Soulbound Instance => instance;
	}
}
