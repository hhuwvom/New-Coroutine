using UnityEngine;
using System.Collections;

public class CoroutineExt {
	public static IEnumerator WaitForAnimatorFinish(Animator animator, int layer) {
		while( true ) {
			AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layer);

			if( stateInfo.normalizedTime >= 1 )
				break;

			yield return null;
		}
	}
}
