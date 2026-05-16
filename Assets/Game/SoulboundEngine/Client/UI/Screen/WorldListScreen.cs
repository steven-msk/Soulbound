using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Client.World;
using SoulboundEngine.Core.Assets;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.UI.Screen {
	public sealed class WorldListScreen : UxmlScreen {
		public const int MAX_WORLDS = 10;
		private readonly IWorldAccessor worldAccessor;
		private readonly VisualTreeAsset worldEntryAsset;
		private int nextWorldIndex;
		private readonly SortedSet<int> removedSlots = new();
		private readonly Dictionary<VisualElement, EventCallback<ClickEvent>> clickCallbacks = new();

		public WorldListScreen(IWorldAccessor worldAccessor) 
			: base(AssetManager.Resolve<VisualTreeAsset>(new AssetKey("WorldListScreen"))) {
			this.worldAccessor = worldAccessor;
			this.worldEntryAsset = AssetManager.Resolve<VisualTreeAsset>(new AssetKey("WorldEntry"));
		}

		protected override void OnBind(VisualElement root) {
			VisualElement worldList = root.Q<VisualElement>("WorldList");
			this.CreateSlots(worldList);
			this.nextWorldIndex = 0;
			int i = 0;

			foreach (var save in this.worldAccessor.ListWorldSaves()) {
				if (this.SpaceAvailable() <= 0) break;

				VisualElement slot = this.GetNextSlot(worldList);
				this.AddWorldToList(save.name, save.seed, slot, i++);
			}

			root.Q<Button>("CreateWorld").clicked += () => {
				TextField nameField = root.Q<TextField>("NameField");
				TextField seedField = root.Q<TextField>("SeedField");

				if (!string.IsNullOrEmpty(nameField.value) && this.SpaceAvailable() > 0) {
					int seed = WorldManager.GetRandomSeed();
					string seedText = seedField.value;
					
					if (!string.IsNullOrEmpty(seedText)) {
						if (!int.TryParse(seedText, out seed)) {
							Logger.LogError("Invalid seed: {}", seedText);
							return;
						}
					}

					this.worldAccessor.CreateNewWorld(nameField.value, seed);

					VisualElement listRoot = root.Q<VisualElement>("WorldList");
					VisualElement slot = this.GetNextSlot(listRoot);
					int index = listRoot.hierarchy.IndexOf(slot);
					this.AddWorldToList(nameField.value, seed, slot, index);
					nameField.value = "";
					seedField.value = "";
				}
			};
		}

		private void CreateSlots(VisualElement listRoot) {
			for (int i = 0; i < MAX_WORLDS; i++) {
				VisualElement slot = this.worldEntryAsset.Instantiate();
				this.ClearSlot(this.GetName(slot), this.GetSeed(slot));
				listRoot.Add(slot);
			}
		}

		private void AddWorldToList(string world, int seed, VisualElement slot, int index) {
			Label nameLabel = this.GetName(slot);
			Label seedLabel = this.GetSeed(slot);

			nameLabel.text = world;
			seedLabel.text = $"Seed: {seed}";
			seedLabel.style.display = DisplayStyle.Flex;

			Button enterWorld = this.GetEnterButton(slot);
			Button deleteWorld = this.GetDeleteButton(slot);

			this.clickCallbacks[enterWorld] = _ => this.worldAccessor.EnterWorld(world);
			this.clickCallbacks[deleteWorld] = _ => {
				this.worldAccessor.DeleteWorld(world);
				this.RemoveWorldFromList(slot, index);
			};

			enterWorld.RegisterCallbackOnce(this.clickCallbacks[enterWorld]);
			deleteWorld.RegisterCallbackOnce(this.clickCallbacks[deleteWorld]);
		}

		private void RemoveWorldFromList(VisualElement slot, int index) {
			Label name = this.GetName(slot);
			Label seed = this.GetSeed(slot);
			this.ClearSlot(name, seed);

			Button enterWorld = this.GetEnterButton(slot);
			Button deleteWorld = this.GetDeleteButton(slot);

			enterWorld.UnregisterCallback(this.clickCallbacks[enterWorld]);
			deleteWorld.UnregisterCallback(this.clickCallbacks[deleteWorld]);

			this.clickCallbacks.Remove(enterWorld);
			this.clickCallbacks.Remove(deleteWorld);

			this.removedSlots.Add(index);
		}

		private void ClearSlot(Label name, Label seed) {
			name.text = "empty";
			seed.text = "";
			seed.style.display = DisplayStyle.None;
		}

		private Label GetName(VisualElement slot) => slot.Q<Label>("WorldName");
		private Label GetSeed(VisualElement slot) => slot.Q<Label>("WorldSeed");

		private Button GetEnterButton(VisualElement slot) => slot.Q<Button>("EnterWorld");
		private Button GetDeleteButton(VisualElement slot) => slot.Q<Button>("DeleteWorld");

		private VisualElement GetNextSlot(VisualElement listRoot) {
			if (this.removedSlots.Any()) {
				int first = this.removedSlots.First();
				this.removedSlots.Remove(first);
				return listRoot[first];
			}

			return listRoot[this.nextWorldIndex++];
		}

		private int SpaceAvailable() => MAX_WORLDS - this.nextWorldIndex;
	}
}
