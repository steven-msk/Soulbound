using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI.Screens {
	[DisallowMultipleComponent]
	public class ChildReference : MonoBehaviour {
		[SerializeField] string overrideAccessor;

		public string accessor => string.IsNullOrEmpty(overrideAccessor)
				? gameObject.name
				: overrideAccessor;
	}
}
