using SoulboundBackend.Client.Input;
using SoulboundBackend.Common;
using SoulboundBackend.Common.Logging;
using SoulboundBackend.Core.AssetManagement;
using SoulboundBackend.Core.Bootstrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.Exceptions;
using Logger = SoulboundBackend.Common.Logging.Logger;

#nullable enable

namespace SoulboundBackend.Core.Resource {
	public static class ResourceManager {
		public static TObject GetAddressableSync<TObject>(AssetKey assetKey) {
			AsyncOperationHandle<TObject> handle = Addressables.LoadAssetAsync<TObject>(assetKey.address);
			handle.WaitForCompletion();

			if (handle.Status == AsyncOperationStatus.Succeeded) {
				return handle.Result;
			} else {
				throw handle.OperationException;
			}
		}

		public static GameObject? GetRuntimePrefab(AssetKey assetKey) {
			return GetAddressableSync<GameObject>(assetKey);
		}
	}
}
