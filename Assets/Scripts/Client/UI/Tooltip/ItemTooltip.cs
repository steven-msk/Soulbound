using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI;
using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#nullable enable

namespace SoulboundBackend.Client.UI {
	[PROTOTYPICAL]
	public class ItemTooltip : ITooltip {
		private readonly string title;
		private readonly string? description;
		private readonly string? lore;

		public ItemTooltip(string title, string? description = null, string? lore = null) {
			this.title = title;
			this.description = description;
			this.lore = lore;
		}

		public ItemTooltip(Item item)
			: this(item.name, "a description", "some lore") {
		}

		public ITooltipHandle Build(ITooltipManager tooltipManager) {
			GameObject obj = new("ItemTooltip", typeof(RectTransform));
			Image bg = obj.AddComponent<Image>();
			bg.sprite = null;
			bg.color = new Color(0.5f, 0.5f, 0.5f, 0.392f);
			bg.raycastTarget = false;
			VerticalLayoutGroup layoutGroup = obj.AddComponent<VerticalLayoutGroup>();
			layoutGroup.padding = new RectOffset(10, 0, 0, 0);
			layoutGroup.childForceExpandWidth = false;
			layoutGroup.childForceExpandHeight = false;
			layoutGroup.childControlWidth = false;
			layoutGroup.childControlHeight = true;
			ContentSizeFitter sizeFitter = obj.AddComponent<ContentSizeFitter>();
			sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
			sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
			RectTransform rect = obj.GetComponent<RectTransform>();
			rect.anchorMin = new Vector2(0f, 0f);
			rect.anchorMin = new Vector2(1f, 1f);
			rect.pivot = new Vector2(0f, 1f);

			GameObject title = new("Title", typeof(RectTransform));
			title.transform.SetParent(obj.transform, false);
			TextMeshProUGUI titleText = title.AddComponent<TextMeshProUGUI>();
			titleText.text = this.title;
			titleText.fontSize = 20;

			if (description != null && !string.IsNullOrEmpty(description)) {
				GameObject description = new("Description", typeof(RectTransform));
				description.transform.SetParent(obj.transform, false);
				TextMeshProUGUI descriptionText = description.AddComponent<TextMeshProUGUI>();
				descriptionText.text = this.description;
				descriptionText.fontSize = 17;
			}

			if (lore != null && !string.IsNullOrEmpty(lore)) {
				GameObject lore = new("Title", typeof(RectTransform));
				lore.transform.SetParent(obj.transform, false);
				TextMeshProUGUI loreText = lore.AddComponent<TextMeshProUGUI>();
				loreText.text = this.lore;
				loreText.fontSize = 17;
				loreText.fontStyle = FontStyles.Italic;
			}

			TooltipHandle handle = obj.AddComponent<TooltipHandle>();
			tooltipManager.AddTooltip(new UITooltipNode(obj, handle));

			return handle;
		}
	}
}
