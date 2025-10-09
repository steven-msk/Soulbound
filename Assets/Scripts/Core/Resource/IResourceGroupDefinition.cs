using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core.Resource {
	public interface IResourceGroupDefinition<TAsset> where TAsset : UnityEngine.Object {
		public string address { get; }
		public string scriptableObjectName { get; }
	}
}