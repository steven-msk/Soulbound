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
using Logger = SoulboundEngine.Client.Debug.Logging.Logger;

namespace SoulboundEngine.Core {
	public sealed class Soulbound : IApplicationController, IDebugMetricsSource {
		private static Soulbound instance;
		private static readonly Logger loggerInstance = new(UnityEngine.Debug.unityLogger);
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

#if !UNITY_EDITOR
			Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
			Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
			Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.None);
#endif

			this.debugMetricsService = new DebugMetricsService();
			this.performanceMetrics = new PerformanceMetrics();
			this.RegisterDebugMetricsSource(this);

			AssetManager.PreloadAll();

			Registries.Init();
			Registries.Freeze();

			this.client = new SoulboundClient(config, new ClientInit {
				debugMetricsService = this.debugMetricsService
			});

			GameStateManager.SetInitialized();
		}

		public void Launch() {
			if (this.running) return;
			GameStateManager.SetLaunching();

			try {
				Thread.CurrentThread.Name = "LaunchThread";
			} catch (InvalidOperationException) {
			}

			Application.quitting += ((IApplicationController)this).OnApplicationQuit;

			UniTask.Post(async () => {
				this.client.Start();

				while (this.running) {
					await UniTask.NextFrame();
					this.Update();
				}
			});

			this.running = true;
			GameStateManager.SetRunning();
		}

		public void Update() {
			this.performanceMetrics.Tick();
			this.client.Update();
		}

		public void CloseGame() => Application.Quit();

		void IApplicationController.OnApplicationQuit() {
			GameStateManager.SetShutdown();

			this.client.Shutdown();
			AssetManager.Shutdown();

			GameStateManager.SetTerminated();
		}

		public void RegisterDebugMetricsSource(IDebugMetricsSource source) {
			this.debugMetricsService.RegisterSource(source);
		}
		public void UnregisterDebugMetricsSource(IDebugMetricsSource source) {
			this.debugMetricsService.UnregisterSource(source);
		}

		void IDebugMetricsSource.CollectDebugData(ref DebugMetricsBuilder builder) {
			builder.Add("fps", this.performanceMetrics.InstantFps);
			builder.Add("frameTime", this.performanceMetrics.FrameTime);
			builder.Add("fixedUpdateTime", this.performanceMetrics.FixedUpdateTime);
			builder.Add("totalManagedMemory", this.performanceMetrics.TotalManagedMemoryMB);
			builder.Add("totalUnityReservedMemory", this.performanceMetrics.TotalUnityReservedMemoryMB);
			builder.Add("monoHeap", this.performanceMetrics.MonoHeapMB);
			builder.Add("monoUsed", this.performanceMetrics.MonoUsedMB);
			builder.Add("gpuManagedMemory", this.performanceMetrics.GPUManagedMemoryMB);
			builder.Add("gpuReservedMemory", this.performanceMetrics.GPUReservedMemoryMB);
			builder.Add("gcAlloc", this.performanceMetrics.GcAllocBytesThisFrame);
		}

		public PerformanceMetrics GetPerformanceMetrics() => this.performanceMetrics;

		public static Soulbound Instance => instance;
	}
}
