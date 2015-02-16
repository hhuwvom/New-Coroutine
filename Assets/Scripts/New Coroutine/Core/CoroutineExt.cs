using UnityEngine;
using System.Collections;

public class CoroutineExt {
	public static IEnumerator WaitForAnimatorFinish(Animator animator, int layer) {
		while( true ) {
			yield return null;

			if( animator.IsInTransition(layer) )
				continue;

			AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layer);

			Debug.Log("state info name is attack ? " + stateInfo.IsName("Attack"));
			Debug.Log("state info name is attack done ? " + stateInfo.IsName("Attack Done"));
			Debug.Log("state info name is idle ? " + stateInfo.IsName("Idle"));

			Debug.Log("animate state info = " + stateInfo.normalizedTime);

			if( stateInfo.normalizedTime >= 1 )
				break;
		}
	}
}
