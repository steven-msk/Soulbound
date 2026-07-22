using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Client.World;
using SoulboundEngine.Core.Assets;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.UI.Screen {
	public sealed class WorldListScreen : UxmlScreen {
		public const int MAX_WORLDS = 10;
		private const string WORLD_LIST_ELEMENT = "WorldList";
		private const string CREATE_WORLD_ELEMENT = "CreateWorld";
		private const string NAME_FIELD_ELEMENT = "NameField";
		private const string SEED_FIELD_ELEMENT = "SeedField";
		private const string WORLD_NAME_FIELD_ELEMENT = "WorldName";
		private const string WORLD_SEED_FIELD_ELEMENT = "WorldSeed";
		private const string ENTER_WORLD_ELEMENT = "EnterWorld";
		private const string DELETE_WORLD_ELEMENT = "DeleteWorld";
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
			VisualElement worldList = root.Q<VisualElement>(WORLD_LIST_ELEMENT);
			this.CreateSlots(worldList);
			this.nextWorldIndex = 0;
			int i = 0;

			foreach (var save in this.worldAccessor.ListWorldSaves()) {
				if (this.SpaceAvailable() <= 0) break;

				VisualElement slot = this.GetNextSlot(worldList);
				this.AddWorldToList(save.name, save.seed, slot, i++);
			}

			root.Q<Button>(CREATE_WORLD_ELEMENT).clicked += () => {
				TextField nameField = root.Q<TextField>(NAME_FIELD_ELEMENT);
				TextField seedField = root.Q<TextField>(SEED_FIELD_ELEMENT);

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

					VisualElement listRoot = root.Q<VisualElement>(WORLD_LIST_ELEMENT);
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

		private Label GetName(VisualElement slot) => slot.Q<Label>(WORLD_NAME_FIELD_ELEMENT);
		private Label GetSeed(VisualElement slot) => slot.Q<Label>(WORLD_SEED_FIELD_ELEMENT);

		private Button GetEnterButton(VisualElement slot) => slot.Q<Button>(ENTER_WORLD_ELEMENT);
		private Button GetDeleteButton(VisualElement slot) => slot.Q<Button>(DELETE_WORLD_ELEMENT);

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
