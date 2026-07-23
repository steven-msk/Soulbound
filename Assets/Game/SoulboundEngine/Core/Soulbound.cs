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
	public sealed class Soulbound : IApplicationController {
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

			Application.quitting += ((IApplicationController)this).OnApplicationQuit;

			UniTask.Post(async () => {
				this.client.Start();

				while (this.running) {
					await UniTask.NextFrame();
					try {
						this.Update();
					} catch (Exception e) {
						// TODO: custom crash handling
						Logger.LogFatal(e);
#if UNITY_EDITOR
						EditorApplication.isPlaying = false;
#else                       
						Environment.FailFast("Uncaught exception in update loop", e);
#endif
					}
				}
			});

			this.running = true;
			GameStateManager.SetRunning();
		}

		public void Update() {
			this.client.Update();
		}

		public void CloseGame() => Application.Quit();

		void IApplicationController.OnApplicationQuit() {
			GameStateManager.SetShutdown();

			this.client.Shutdown();
			AssetManager.Shutdown();

			GameStateManager.SetTerminated();
		}

		public static Soulbound Instance => instance;
	}
}
