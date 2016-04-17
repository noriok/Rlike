using UnityEngine;
// using System.Collections;

public class BombFireAnimationFinished : StateMachineBehaviour {
	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		// Debug.Log("爆発アニメ Start");
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		// Debug.Log("OnStateUpdate");
		Debug.Log(stateInfo.normalizedTime);

		if (stateInfo.normalizedTime >= 1) {
			Debug.Log("爆発アニメ 終了");
			Destroy(animator.transform.root.gameObject);
		}
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		// OnStateExit で Destroy すると、ステート開始のアニメが 1 フレーム(?) だけ見えてしまう。
		// Destroy(animator.transform.root.gameObject);
	}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		// Debug.Log("OnStateMove");
	}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
