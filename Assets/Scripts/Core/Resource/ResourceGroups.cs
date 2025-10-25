using SoulboundBackend.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Logger = SoulboundBackend.Common.Logging.Logger;

namespace SoulboundBackend.Core.Resource {
	public static partial class ResourceGroups {
		public static class Runtime {
			const string parentAddress = "runtime/";

			public sealed class Prefabs : IResourceGroupDefinition<GameObject> {
				public static readonly Prefabs instance = new();
				public static void Register() => ResourceManager.RegisterGroupDefinition<Prefabs, GameObject>(instance);
				public string address => $"{Runtime.parentAddress}prefabs/";
				public string scriptableObjectName => "runtimeGroup";
            }
		}

		public static class Items {
			const string parentAddress = "items/";

			public sealed class Icons : IResourceGroupDefinition<Sprite> {
				public static readonly Icons instance = new();
				public static void Register() => ResourceManager.RegisterGroupDefinition<Icons, Sprite>(instance);
				public string address => Items.parentAddress + "icons/";
				public string scriptableObjectName => "iconsGroup";
            }
		}

		public sealed class Tiles : IResourceGroupDefinition<TileBase> {
			public static readonly Tiles instance = new();
			public static void Register() => ResourceManager.RegisterGroupDefinition<Tiles, TileBase>(instance);
			public string address => "tiles/";
			public string scriptableObjectName => "tilesGroup";
        }

		public sealed class Prefabs : IResourceGroupDefinition<GameObject> {
			public static readonly Prefabs instance = new();
			public static void Register() => ResourceManager.RegisterGroupDefinition<Prefabs, GameObject>(instance);
			public string address => "prefabs/";
			public string scriptableObjectName => "prefabsGroup";
        }

		public sealed class Fonts : IResourceGroupDefinition<TMP_FontAsset> {
			public static readonly Fonts instance = new();
			public static void Register() => ResourceManager.RegisterGroupDefinition<Fonts, TMP_FontAsset>(instance);
			public string address => "fonts/";
			public string scriptableObjectName => "fontsGroup";
        }
	}
}
