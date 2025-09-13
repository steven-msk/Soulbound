using SoulboundBackend.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using Logger = SoulboundBackend.Common.Logging.Logger;

namespace SoulboundBackend.Core.Resource {
	public static class ResourceGroups {
		private static readonly Logger logger = Logger.CreateInstance();
		static Dictionary<Type, string> addressesByGroupType = new();
		static Dictionary<string, ResourceGroup> groupsByAddress = new();

		public static void Bootstrap() {
			logger.LogInfo(LogModules.resource, "Bootstrapping resource group types");
			Resources.LoadAll<ResourceGroup>("Resources/");
			var groupTypes = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(a => a.GetTypes())
				.Where(t => t.IsClass && !t.IsAbstract)
				.Where(t => t.GetInterfaces().Any(i =>
					i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IResourceGroupDefinition<>)));
			foreach (var type in groupTypes) {
				System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
			}
		}

		public static string GetAddressFromGroupDefinition<TGroup>() => addressesByGroupType[typeof(TGroup)];

		static void RegisterGroupDefinition<TGroup, TAsset>(TGroup group)
				where TAsset : UnityEngine.Object
				where TGroup : IResourceGroupDefinition<TAsset> {
			if (addressesByGroupType.TryAdd(typeof(TGroup), group.address)) {
				logger.LogInfo(LogModules.resource, "Registered resource group definition '{}' of type {}", group.address, typeof(TGroup));
			}
		}

		public static void RegisterGroupByAddress(ResourceGroup group) => groupsByAddress.Add(group.groupAddress, group);

		public static ResourceGroup GetGroupByAddress(string address) {
			return groupsByAddress[address];
		}

		public static class Items {
			private static string parentAddress = "items/";

			public sealed class Icons : IResourceGroupDefinition<Sprite> {
				public static readonly Icons instance = new();
				static Icons() => RegisterGroupDefinition<Icons, Sprite>(instance);
				public string address => Items.parentAddress + "icons/";
			}
		}

		public sealed class Tiles : IResourceGroupDefinition<TileBase> {
			public static readonly Tiles instance = new();
			static Tiles() => RegisterGroupDefinition<Tiles, TileBase>(instance);
			public string address => "tiles/";
		}

		public sealed class Prefabs : IResourceGroupDefinition<GameObject> {
			public static readonly Prefabs instance = new();
			static Prefabs() => RegisterGroupDefinition<Prefabs, GameObject>(instance);
			public string address => "prefabs/";
		}

		public sealed class Fonts : IResourceGroupDefinition<TMP_FontAsset> {
			public static readonly Fonts instance = new();
			static Fonts() => RegisterGroupDefinition<Fonts, TMP_FontAsset>(instance);
			public string address => "fonts/";
		}

		public interface IResourceGroupDefinition<TAsset> where TAsset : UnityEngine.Object {
			public string address { get; }
		}
	}
}
