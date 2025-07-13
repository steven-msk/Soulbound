using TMPro;
using UnityEngine;

public class DebugCoordinatesUpdater : MonoBehaviour {
	private void Update() {
		Vector2 worldPos = GameManager.instance.Player.position;
		int chunkX = GameManager.instance.Level.ChunkXAt(worldPos);
		string formatted = $"XY: ({worldPos.x.ToString("F2")}, {worldPos.y.ToString("F2")}) c: {chunkX}";
		this.GetComponent<TextMeshProUGUI>().text = formatted;
	}
}