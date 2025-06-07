using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public static bool IsPaused { get; private set; }

	public static PlayerController GetPlayerInstance() => GameObject.Find("johnny").GetComponent<PlayerController>();

	public static UIController GetUI() => GameObject.Find("Canvas").GetComponent<UIController>();

	private void Awake() {
		Registry.RegisterAll<TMP_FontAsset>("Registry/Fonts");
		Registry.RegisterAll<Item>("Registry/Items");
		Registry.RegisterAll<GameObject>("Registry/Prefabs");
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.P)) {
			IsPaused = !IsPaused;
			Time.timeScale = IsPaused ? 0f : 1f;
		}
		
	}

}