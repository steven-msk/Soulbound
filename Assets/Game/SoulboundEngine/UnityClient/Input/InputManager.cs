using System;
using UnityEngine.InputSystem;

namespace SoulboundEngine.UnityClient.Input {
	public sealed class InputManager : IDisposable {
		private readonly InputActionAsset asset;
		public readonly Mouse mouse;
		public readonly Keyboard keyboard;

		public InputManager(InputActionAsset asset) {
			this.asset = asset;
			this.mouse = new Mouse(asset);
			this.keyboard = new Keyboard(asset);
		}

		internal void Tick() {
			this.mouse.Tick();
			this.keyboard.Tick();
		}

		public void Enable() {
			this.mouse.Enable();
			this.keyboard.Enable();
		}

		public void Disable() {
			this.mouse.Disable();
			this.keyboard.Disable();
		}

		public void Dispose() {
			this.mouse.Dispose();
			this.keyboard.Dispose();
		}

		public InputActionAsset GetAsset() => this.asset;

		public abstract class MappedInputActions : IDisposable {
			protected readonly InputActionMap map;

			public MappedInputActions(InputActionAsset asset) {
				this.map = this.GetMap(asset);
			}

			protected abstract InputActionMap GetMap(InputActionAsset asset);

			internal protected abstract void Tick();

			public void Enable() => this.map.Enable();
			public void Disable() => this.map.Disable();
			public void Dispose() => this.map.Dispose();
		}

	}
}
