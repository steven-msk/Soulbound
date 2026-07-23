using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Client.UI.UXMLBindings;
using SoulboundEngine.Client.World;
using SoulboundEngine.Core.Assets;
using SoulboundEngine.Core.Registry;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.UI.Screen {
	public sealed class WorldListScreen : UXMLScreen {
		public const int MAX_WORLDS = 10;
		private static readonly Identifier WORLD_LIST_ELEMENT = Identifier.Of("soulbound:world_list_screen/world_list");
		private static readonly Identifier CREATE_WORLD_ELEMENT = Identifier.Of("soulbound:world_list_screen/create_world");
		private static readonly Identifier NAME_FIELD_ELEMENT = Identifier.Of("soulbound:world_list_screen/name_field");
		private static readonly Identifier SEED_FIELD_ELEMENT = Identifier.Of("soulbound:world_list_screen/seed_field");
		private static readonly Identifier WORLD_NAME_FIELD_ELEMENT = Identifier.Of("soulbound:world_entry/world_name");
		private static readonly Identifier WORLD_SEED_FIELD_ELEMENT = Identifier.Of("soulbound:world_entry/world_seed");
		private static readonly Identifier ENTER_WORLD_ELEMENT = Identifier.Of("soulbound:world_entry/enter_world");
		private static readonly Identifier DELETE_WORLD_ELEMENT = Identifier.Of("soulbound:world_entry/delete_world");
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
			VisualElement worldList = root.Get<VisualElement>(WORLD_LIST_ELEMENT);
			this.CreateSlots(worldList);
			this.nextWorldIndex = 0;
			int i = 0;

			foreach (var save in this.worldAccessor.ListWorldSaves()) {
				if (this.SpaceAvailable() <= 0) break;

				VisualElement slot = this.GetNextSlot(worldList);
				this.AddWorldToList(save.name, save.seed, slot, i++);
			}

			root.Get<Button>(CREATE_WORLD_ELEMENT).clicked += () => {
				TextField nameField = root.Get<TextField>(NAME_FIELD_ELEMENT);
				TextField seedField = root.Get<TextField>(SEED_FIELD_ELEMENT);

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

					VisualElement listRoot = root.Get<VisualElement>(WORLD_LIST_ELEMENT);
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

		private Label GetName(VisualElement slot) => slot.Get<Label>(WORLD_NAME_FIELD_ELEMENT);
		private Label GetSeed(VisualElement slot) => slot.Get<Label>(WORLD_SEED_FIELD_ELEMENT);

		private Button GetEnterButton(VisualElement slot) => slot.Get<Button>(ENTER_WORLD_ELEMENT);
		private Button GetDeleteButton(VisualElement slot) => slot.Get<Button>(DELETE_WORLD_ELEMENT);

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
