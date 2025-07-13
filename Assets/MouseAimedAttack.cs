using UnityEngine;
using UnityEngineInternal;

public class MouseAimedAttack : StateMachineBehaviour {
	public float speed;
	public float horizontalAngleOffset;
	private Vector2 direction;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		Vector3 mousePos = GameManager.instance.Player.InputHandler.MouseWorldPosition;
		direction = (mousePos - animator.transform.position).normalized;
		animator.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + horizontalAngleOffset);
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		animator.transform.position += speed * Time.deltaTime * (Vector3)direction;
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