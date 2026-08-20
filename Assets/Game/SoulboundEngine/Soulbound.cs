using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SoulboundEngine.Client;
using SoulboundEngine.Client.Assets;
using SoulboundEngine.Common.Json;
using SoulboundEngine.GameStates;
using SoulboundEngine.Registry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEditor;
using UnityEngine;
using Logger = SoulboundEngine.Client.Debug.Logging.Logger;

namespace SoulboundEngine {
	public sealed class Soulbound {
		public const float TICK_RATE = 1f / SharedConstants.TICKS_PER_SECOND;
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
		private float tickStartTime;
		private readonly SoulboundClient client;
		public readonly GameConfig config;
		private readonly Stopwatch tickStopwatch = new();
		private readonly Stopwatch tpsWindowStopwatch = new();
		private int ticksThisSecond;

		public Soulbound(GameConfig config) {
			instance = this;
			this.config = config;
			GameStateManager.SetBootstrapping();

			AssetManager.LoadAllWithPreloadLabel();

			Registries.Init();
			Registries.Freeze();

			this.client = new SoulboundClient(config);

			GameStateManager.SetInitialized();
		}

		public void Launch() {
			if (this.running) return;
			GameStateManager.SetLaunching();

			try {
				Thread.CurrentThread.Name = "LaunchThread";
			} catch (InvalidOperationException) {
			}

			Application.quitting += this.OnApplicationQuit;

			this.running = true;
			this.client.Start();
			UniTask.Post(this.UpdateLoop);
			UniTask.Post(this.TickLoop);

			GameStateManager.SetRunning();
		}

		private async void UpdateLoop() {
			while (this.running) {
				try {
					this.client.Update();
				} catch (Exception e) {
					// TODO: custom crash handling
					Logger.LogFatal(e);
#if UNITY_EDITOR
					EditorApplication.isPlaying = false;
#else
					Environment.FailFast("Uncaught exception in update loop", e);
#endif
				}
				await UniTask.NextFrame();
			}
		}

		private async void TickLoop() {
			while (this.running) {
				try {
					this.StartTick();
					this.client.Tick();
					this.EndTick();
				} catch (Exception e) {
					Logger.LogFatal(e);
#if UNITY_EDITOR
					EditorApplication.isPlaying = false;
#else
					Environment.FailFast("Uncaught exception in tick loop", e);
#endif
				}
				await UniTask.WaitForSeconds(TICK_RATE, true);
			}
		}

		private void StartTick() {
			this.tickStopwatch.Restart();
			this.tickStartTime = Time.realtimeSinceStartup;
		}

		private void EndTick() {
			this.tickStopwatch.Stop();
			double elapsedMs = this.tickStopwatch.Elapsed.TotalMilliseconds;
			if (elapsedMs > TICK_RATE * 1000f * 1.5f) {
				Logger.LogWarning($"Tick lag detected! Tick took {elapsedMs:F1}ms (target {TICK_RATE * 1000F}ms)");
			}

			this.ticksThisSecond++;
			if (this.tpsWindowStopwatch.Elapsed.TotalSeconds >= 1.0d) {
				if (this.ticksThisSecond < SharedConstants.TICKS_PER_SECOND - 1) {
					Logger.LogWarning("Tick rate degraded: {} TPS (target {})", this.ticksThisSecond, SharedConstants.TICKS_PER_SECOND);
				}
				this.ticksThisSecond = 0;
				this.tpsWindowStopwatch.Restart();
			}
		}

		public void CloseGame() => Application.Quit();

		private void OnApplicationQuit() {
			GameStateManager.SetShutdown();

			this.client.Shutdown();
			AssetManager.Shutdown();

			GameStateManager.SetTerminated();
		}

		public static Soulbound Instance => instance;
	}
}
