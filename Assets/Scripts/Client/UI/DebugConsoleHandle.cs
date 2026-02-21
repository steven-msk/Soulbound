using SoulboundBackend.Core.Debug;
using SoulboundBackend.Core.Debug.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoulboundBackend.Client.UI {
	public sealed class DebugConsoleHandle : MonoBehaviour, IDebugConsoleHandle {
		public void AddLogEntry(LogEntry entry) {
			CreateLog(entry);
		}

		private void CreateLog(LogEntry entry) {
			GameObject obj = new("Log Entry", typeof(RectTransform));
			obj.transform.SetParent(transform, false);

			TextMeshProUGUI text = obj.AddComponent<TextMeshProUGUI>();
			text.fontSize = 12f;
			text.text = entry.message;

			ContentSizeFitter sizeFitter = obj.AddComponent<ContentSizeFitter>();
			sizeFitter.verticalFit = sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
		}

		public void SetVisible(bool visible) {
			throw new NotImplementedException();
		}
	}
}
