using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.ContentSizeFitter;

namespace SoulboundBackend.Client.SettingSystem {
	[PROTOTYPICAL]
	public class SettingContainerBuilder {
		public const float DEFAULT_HORIZONTAL_SPACING = 8f;
		public const ContentSizeFitter.FitMode DEFAULT_FIT_MODE = FitMode.PreferredSize;

		public readonly AbstractSettingEntry entry;
		public readonly SettingEntryGroup group;
		private GameObject container;
		private GameObject nameObject;

		public SettingContainerBuilder(SettingEntryGroup group, AbstractSettingEntry entry) {
			this.group = group;
			this.entry = entry;
		}

		public GameObject ConstructContainer() {
			if (container != null) {
				throw new InvalidOperationException($"Setting container already constructed for setting '{entry}'");
			}
			this.container = new("Setting Container", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(ContentSizeFitter));
			container.transform.SetParent(group.transform, false);
			var layout = container.GetComponent<HorizontalLayoutGroup>();
			layout.spacing = DEFAULT_HORIZONTAL_SPACING;
			layout.childControlWidth = layout.childControlHeight = false;
			layout.childForceExpandWidth = layout.childForceExpandHeight = false;
			layout.childScaleWidth = layout.childScaleHeight = true;
			layout.childAlignment = TextAnchor.MiddleLeft;
			var sizeFitter = container.GetComponent<ContentSizeFitter>();
			sizeFitter.horizontalFit = sizeFitter.verticalFit = DEFAULT_FIT_MODE;

			this.nameObject = new("Name", typeof(RectTransform), typeof(TextMeshProUGUI));
			nameObject.transform.SetParent(container.transform, false);
			TextMeshProUGUI name = nameObject.GetComponent<TextMeshProUGUI>();
			name.fontSize = 15f;
			name.alignment = TextAlignmentOptions.MidlineRight;
			name.autoSizeTextContainer = true;
			name.SetText($"{entry.displayName}:");

			return container;
		}
	}
}
