using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	[Header("Flight Time Panel")]
	[SerializeField] private GameObject flightTimePanel;
	[SerializeField] private float maskWidth;               // maskWidth must be equal to TimeBar width (center&middle anchor)
	
	public void UpdateFlightTimePanel(bool isFlying, float flightTime, float grantedFlightTime) {
		flightTimePanel.SetActive(isFlying && GameManager.GetPlayerInstance().InputHandler.PressingSpace);
		RectMask2D timeMask = flightTimePanel.GetComponentInChildren(typeof(RectMask2D), true) as RectMask2D;
		timeMask.padding = new Vector4(maskWidth * (1 - flightTime / grantedFlightTime), 0, 0, 0);
	}
}