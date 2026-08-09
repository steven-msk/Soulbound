using SoulboundEngine.Common;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.ContentSizeFitter;

namespace SoulboundEngine.Client.Settings.View {
	[PROTOTYPICAL]
	public class SettingContainerBuilder {
		public const float DEFAULT_HORIZONTAL_SPACING = 8f;
		public const ContentSizeFitter.FitMode DEFAULT_FIT_MODE = FitMode.PreferredSize;

		public readonly SettingEntry entry;
		public readonly SettingEntryGroup group;
		private GameObject container;
		private GameObject nameObject;

		public SettingContainerBuilder(SettingEntryGroup group, SettingEntry entry) {
			this.group = group;
			this.entry = entry;
		}

		public GameObject ConstructContainer() {
			if (this.container != null) {
				throw new InvalidOperationException($"Setting container already constructed for setting '{this.entry}'");
			}
			this.container = new("Setting Container", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(ContentSizeFitter));
			this.container.transform.SetParent(this.group.transform, false);
			var layout = this.container.GetComponent<HorizontalLayoutGroup>();
			layout.spacing = DEFAULT_HORIZONTAL_SPACING;
			layout.childControlWidth = layout.childControlHeight = false;
			layout.childForceExpandWidth = layout.childForceExpandHeight = false;
			layout.childScaleWidth = layout.childScaleHeight = true;
			layout.childAlignment = TextAnchor.MiddleLeft;
			var sizeFitter = this.container.GetComponent<ContentSizeFitter>();
			sizeFitter.horizontalFit = sizeFitter.verticalFit = DEFAULT_FIT_MODE;

			this.nameObject = new("Name", typeof(RectTransform), typeof(TextMeshProUGUI));
			this.nameObject.transform.SetParent(this.container.transform, false);
			TextMeshProUGUI name = this.nameObject.GetComponent<TextMeshProUGUI>();
			name.fontSize = 15f;
			name.alignment = TextAlignmentOptions.MidlineRight;
			name.autoSizeTextContainer = true;
			name.SetText($"{this.entry.id}:");

			return this.container;
		}
	}
}
