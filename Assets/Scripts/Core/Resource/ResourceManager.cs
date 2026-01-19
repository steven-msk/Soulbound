using SoulboundBackend.Client.Input;
using SoulboundBackend.Common;
using SoulboundBackend.Common.Logging;
using SoulboundBackend.Core.Bootstrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Logger = SoulboundBackend.Common.Logging.Logger;

#nullable enable

namespace SoulboundBackend.Core.Resource {
    // see pre-production.md
    [PROTOTYPICAL]
    [Obsolete]
	public static class ResourceManager {
        private static readonly Logger logger = Logger.CreateInstance();
        public static bool groupsPreloaded { get; private set; } = false;
        public static bool definitionsRegistered { get; private set; } = false;
        static Dictionary<Type, object> registeredGroupDefinitions = new();
        static Dictionary<Type, string> addressesByGroupType = new();
        static Dictionary<string, ResourceGroup> groupsByAddress = new();

        public static void PreloadGroups() {
            if (groupsPreloaded) {
                logger.LogInfo(LogModules.resource, "Resource group types already bootstrapped, skipping"); 
                return;
            }

            logger.LogInfo(LogModules.resource, "Bootstrapping resource group types");
            ResourceGroup[] groups = Resources.LoadAll<ResourceGroup>("");
            logger.LogInfo(LogModules.resource, "Found {} resource group instances", groups.Length);
            foreach (var group in groups) {
                groupsByAddress.TryAdd(group.groupAddress, group);
            }
            RegisterDefinitions();
            
            groupsPreloaded = true;
        }

        public static void RegisterDefinitions() {
            if (definitionsRegistered) {
                logger.LogInfo(LogModules.resource, "Resource group definitions already registered, skipping");
                return;
            }

            var groupTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract)
                .Where(t => t.GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IResourceGroupDefinition<>)));
            foreach (var type in groupTypes) {
                var registerMethod = type.GetMethod("Register", BindingFlags.Public | BindingFlags.Static);
                registerMethod?.Invoke(null, null);
            }

            definitionsRegistered = true;
        }


        public static TAsset? Get<TAsset, TGroup>(string name)
				where TAsset : UnityEngine.Object
				where TGroup : IResourceGroupDefinition<TAsset> {
            string address = "";
            ResourceGroup group = null!;
            try {
                GetAddressFromGroupDefinition<TGroup>();
            } catch (KeyNotFoundException) {
                RegisterDefinitions();
            } finally {
                address = GetAddressFromGroupDefinition<TGroup>();
            }

            try {
                group = GetGroupByAddress(address);
            } catch (KeyNotFoundException) {
                if (registeredGroupDefinitions.TryGetValue(typeof(TGroup), out var registeredGroup)
                        && registeredGroup is IResourceGroupDefinition<TAsset> groupDefinition) {
                    group = Resources.Load<ResourceGroup>(groupDefinition.scriptableObjectName);
                    RegisterGroupByAddress(group);
                } else {
                    throw new InvalidOperationException($"Unknown group type {typeof(TGroup)}");
                }
            }

			return group.GetAsset<TAsset>(name);
		}

        public static string GetAddressFromGroupDefinition<TGroup>() {
            return addressesByGroupType[typeof(TGroup)];
        }

        public static void RegisterGroupDefinition<TGroup, TAsset>(TGroup group)
                where TAsset : UnityEngine.Object
                where TGroup : IResourceGroupDefinition<TAsset> {
            if (addressesByGroupType.TryAdd(typeof(TGroup), group.address)
                    && registeredGroupDefinitions.TryAdd(typeof(TGroup), group)) {
                logger.LogInfo(LogModules.resource, "Registered resource group definition '{}' of type {}", group.address, typeof(TGroup));
            }
        }

        public static void RegisterGroupByAddress(ResourceGroup group) {
            groupsByAddress.TryAdd(group.groupAddress, group);
        }

        public static ResourceGroup GetGroupByAddress(string address) {
            return groupsByAddress[address];
        }

        public static void UnloadGroup(ResourceGroup group, Type groupType) {
            groupsByAddress.Remove(group.groupAddress);
            addressesByGroupType.Remove(groupType);
        }

        public static void UnloadAll() {
            groupsByAddress.Clear();
            groupsPreloaded = false;
        }

        public static void ClearDefinitions() {
            addressesByGroupType.Clear();
            definitionsRegistered = false;
        }

        public static GameObject? GetRuntimePrefab(string name) {
            return Get<GameObject, ResourceGroups.Runtime.Prefabs>(name);
        }
    }
}
