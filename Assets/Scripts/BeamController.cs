using UnityEngine;

public class BeamController : MonoBehaviour {
	public Animator animator;
	public Rigidbody2D rb;
	public float beamSpeed;
	public float maxDistance;
	public Vector2 facing;
	private float distance;

	private void Awake() {
		gameObject.GetComponent<Rigidbody2D>().linearVelocity = beamSpeed * facing;
	}

	private void Update() {
		if (Mathf.Abs(distance) >= maxDistance) {
			Destroy(gameObject);
		} else {
			Vector2 facingUnit = beamSpeed * Time.deltaTime * facing;
			distance += facingUnit.magnitude;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.CompareTag("Enemy") || collision.CompareTag("Ground")) {
			Destroy(gameObject);
		}
	}
}