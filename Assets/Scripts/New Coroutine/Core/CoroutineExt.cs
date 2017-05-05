using UnityEngine;
using System.Collections;

public class CoroutineExt {
	public delegate bool ConditionBlock();
	public delegate void ConditionCompletion(bool result);

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

	public static IEnumerator WaitForCondition(ConditionBlock block, ConditionCompletion complete = null, bool condition = true, float waitTime = 10.0f) {
		if( block == null )
			yield break;

		float time = Time.time;
		bool flag = false;

		while( true ) {
			if( waitTime > 0 && Time.time - time > waitTime )
				break;

			if( block() == condition ) {
				flag = true;
				break;
			}

			yield return null;
		}

		if( complete != null )
			complete(flag);
	}
}
