using SoulboundBackend.Core.Resource;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Logger = SoulboundBackend.Common.Logging.Logger;

namespace SoulboundBackend.Client.UI.Tooltip {
	public class TooltipRenderer {
		public delegate TooltipNodeStyle NodeStyleFactory(TooltipNode node);

		private static readonly Logger logger = Logger.CreateInstance();
		public NodeStyleFactory styleFactory;

		public TooltipRenderer(NodeStyleFactory styleProvider) {
			this.styleFactory = styleProvider;
		}

		public GameObject Render(TooltipData data, Vector2 position, Transform parent) {
			if (data == null) {
				logger.ThrowException(null, new ArgumentException("Unable to render tooltip with null data"));
				return null;
			}
			data.PurgeInvalidNodes();
			GameObject panel = GameObject.Instantiate(GetTooltipPrefab("tooltipPanel"), parent, false);
			Transform panelTransform = panel.GetComponent<Transform>();

			foreach (var dataNode in data.nodes) {
				GameObject nodeObj = GameObject.Instantiate(GetTooltipPrefab("tooltipSection"), panelTransform);
				TextMeshProUGUI node = nodeObj.GetComponent<TextMeshProUGUI>();

				node.text = dataNode.text;
				TooltipNodeStyle nodeStyle = styleFactory.Invoke(dataNode.node);
				nodeStyle.Apply(node);
			}
			data.layout.Apply(panel.GetComponent<VerticalLayoutGroup>());
			panelTransform.position = position;
			return panel;
		}

		private static GameObject GetTooltipPrefab(string name) {
			return ResourceManager.Get<GameObject, ResourceGroups.Prefabs>(name);
		}
	}
}
