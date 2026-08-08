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

			UniTask.Post(this.StartUpdate);
			UniTask.Post(this.StartTick);

			this.running = true;
			GameStateManager.SetRunning();
		}

		private async void StartUpdate() {
			this.client.Start();

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

		private async void StartTick() {
			while (this.running) {
				try {
					this.client.Tick();
				} catch (Exception e) {
					Logger.LogFatal(e);
#if UNITY_EDITOR
					EditorApplication.isPlaying = false;
#else
					Environment.FailFast("Uncaught exception in tick loop", e);
#endif
				}
				await UniTask.WaitForSeconds(SharedConstants.TICKS_PER_SECOND, true);
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
