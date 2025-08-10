using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class ResourceGroups {
	static Dictionary<Type, string> addressesByGroupType = new();
	static Dictionary<string, ResourceGroup> groupsByAddress = new();

	public static void Bootstrap() {
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
			Debug.Log($"Registered resource group definition '{group.address}' of type {typeof(TGroup)}");
		}
	}

	public static void RegisterGroupByAddress(ResourceGroup group) => groupsByAddress.Add(group.groupAddress, group);

	public static ResourceGroup GetGroupByAddress(string address) => groupsByAddress[address]; 

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
