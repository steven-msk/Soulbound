namespace SoulboundEngine.UnityClient.UI.Screen {
	using SoulboundEngine.UnityClient.Assets;
	using SoulboundEngine.UnityClient.UI.UXMLBindings;
	using SoulboundEngine.World;
	using SoulboundEngine.World.Serialization;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine.UIElements;

	public sealed class WorldListScreen : UXMLScreen {
		public const int MAX_WORLDS = 10;
		private static readonly UXMLBinding<VisualElement> WORLD_LIST_ELEMENT = new("soulbound:world_list_screen/world_list");
		private static readonly UXMLBinding<Button> CREATE_WORLD_ELEMENT = new("soulbound:world_list_screen/create_world");
		private static readonly UXMLBinding<TextField> NAME_FIELD_ELEMENT = new("soulbound:world_list_screen/name_field");
		private static readonly UXMLBinding<TextField> SEED_FIELD_ELEMENT = new("soulbound:world_list_screen/seed_field");
		private static readonly UXMLBinding<Label> WORLD_NAME_FIELD_ELEMENT = new("soulbound:world_entry/world_name");
		private static readonly UXMLBinding<Label> WORLD_SEED_FIELD_ELEMENT = new("soulbound:world_entry/world_seed");
		private static readonly UXMLBinding<Button> ENTER_WORLD_ELEMENT = new("soulbound:world_entry/enter_world");
		private static readonly UXMLBinding<Button> DELETE_WORLD_ELEMENT = new("soulbound:world_entry/delete_world");
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
			VisualElement worldList = WORLD_LIST_ELEMENT.Get(root);
			this.CreateSlots(worldList);
			this.nextWorldIndex = 0;
			int i = 0;

			foreach (WorldSave save in this.worldAccessor.ListWorldSaves()) {
				if (this.SpaceAvailable() <= 0) break;

				VisualElement slot = this.GetNextSlot(worldList);
				this.AddWorldToList(save.name, save.seed, slot, i++);
			}

			CREATE_WORLD_ELEMENT.Get(root).clicked += () => {
				TextField nameField = NAME_FIELD_ELEMENT.Get(root);
				TextField seedField = SEED_FIELD_ELEMENT.Get(root);

				if (!string.IsNullOrEmpty(nameField.value) && this.SpaceAvailable() > 0) {
					int seed = SoulboundUnityClient.GetRandomWorldSeed();
					string seedText = seedField.value;
					
					if (!string.IsNullOrEmpty(seedText)) {
						if (!int.TryParse(seedText, out seed)) {
							Logger.LogError("Invalid seed: {}", seedText);
							return;
						}
					}

					this.worldAccessor.CreateNewWorld(nameField.value, seed);

					VisualElement listRoot = WORLD_LIST_ELEMENT.Get(root);
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

		private Label GetName(VisualElement slot) => WORLD_NAME_FIELD_ELEMENT.Get(slot);
		private Label GetSeed(VisualElement slot) => WORLD_SEED_FIELD_ELEMENT.Get(slot);

		private Button GetEnterButton(VisualElement slot) => ENTER_WORLD_ELEMENT.Get(slot);
		private Button GetDeleteButton(VisualElement slot) => DELETE_WORLD_ELEMENT.Get(slot);

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
