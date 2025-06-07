using UnityEngine;

public class CameraMovement : MonoBehaviour {

    void Update() {
		Vector3 playerPos = GameObject.Find("johnny").GetComponentInChildren<Transform>().position;
		gameObject.transform.position = new Vector3(playerPos.x, playerPos.y, -1);

	}
}
