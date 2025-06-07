using UnityEngine;

public class BeamCallback : StateMachineBehaviour {
	private BeamController controller;
	private Vector2 facing;
	private float distance;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		controller = animator.gameObject.GetComponent<BeamController>();
		facing = controller.facing;
		distance = 0;
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (Mathf.Abs(distance) >= controller.maxDistance * controller.beamSpeed) {
			Destroy(animator.gameObject);
		} else {
			Vector2 facingUnit = controller.beamSpeed * Time.deltaTime * facing;
			distance += facingUnit.magnitude;
		}
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	//override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	//{
	//
	//}

	// OnStateMove is called right after Animator.OnAnimatorMove()
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	//{
	//    // Implement code that processes and affects root motion
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK()
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	//{
	//    // Implement code that sets up animation IK (inverse kinematics)
	//}
}