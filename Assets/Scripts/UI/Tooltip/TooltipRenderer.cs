using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class TooltipRenderer {
	public delegate TooltipNodeStyle NodeStyleProvider(TooltipNode node);

	private static readonly Logger logger = Logger.CreateInstance();
	public NodeStyleProvider styleProvider;

	public TooltipRenderer(NodeStyleProvider styleProvider) {
		this.styleProvider = styleProvider;
	}

	public GameObject Render(TooltipData data, Vector2 position, Transform parent) {
		if (data == null) {
			logger.ThrowException(null, new ArgumentException("Unable to render tooltip with null data"));
			return null;
		}
		GameObject panel = GameObject.Instantiate(GetTooltipPrefab("tooltipPanel"), parent, false);
		Transform panelTransform = panel.GetComponent<Transform>();

		foreach (var dataNode in data.nodes) {
			GameObject nodeObj = GameObject.Instantiate(GetTooltipPrefab("tooltipSection"), panelTransform);
			TextMeshProUGUI node = nodeObj.GetComponent<TextMeshProUGUI>();

			node.text = dataNode.text;
			TooltipNodeStyle nodeStyle = styleProvider.Invoke(dataNode.node);
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
