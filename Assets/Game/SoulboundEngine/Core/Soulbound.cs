using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SoulboundEngine.Client;
using SoulboundEngine.Common.Json;
using SoulboundEngine.Core.Assets;
using SoulboundEngine.Core.GameStates;
using SoulboundEngine.Core.Registry;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using Logger = SoulboundEngine.Client.Debug.Logging.Logger;

namespace SoulboundEngine.Core {
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
			this.tickStartTime = Time.realtimeSinceStartup;
		}

		private void EndTick() {
			float elapsed = Time.realtimeSinceStartup - this.tickStartTime;
			if (elapsed > SharedConstants.TICKS_PER_SECOND) {
				Logger.LogWarning($"Tick lag detected! Tick took {elapsed * 1000f:F1} ms");
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
