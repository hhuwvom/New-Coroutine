using UnityEngine;
using System.Collections;

public class CoroutineTest : MonoBehaviour {
	#region Coroutine
	public IEnumerator RotateZTest(float angle, float time, Transform moveObj = null) {
		if( moveObj == null )
			moveObj = transform;

		float fromeAngle = moveObj.localEulerAngles.z;
		float nowTime = 0;

		while( nowTime < time ) {
			nowTime = Mathf.Min(nowTime + Time.deltaTime, time);

			Vector3 nowAngle = moveObj.localEulerAngles;

			nowAngle.z = fromeAngle + (angle * nowTime / time);

			moveObj.localEulerAngles = nowAngle;

			yield return null;
		}
	}

	public IEnumerator MoveTest(Vector3 move, float time, Transform moveObj = null) {
		if( moveObj == null )
			moveObj = transform;

		Vector3 fromPosition = moveObj.localPosition;
		float nowTime = 0;

		while( nowTime < time ) {
			nowTime = Mathf.Min(nowTime + Time.deltaTime, time);

			Vector3 nowPosition = moveObj.localPosition;

			nowPosition = fromPosition + (move * nowTime / time);

			moveObj.localPosition = nowPosition;

			yield return null;
		}
	}

	public IEnumerator MoveAndRotateTest(Vector3 move, float angle, float time, Transform moveObj = null) {
		yield return MoveTest(move, time, moveObj);

		// Don't use new WaitForSeconds(), it does not support.
		Debug.Log("Wait for 3 seconds.");

		yield return 3.0f;

		// or you can use "yield return new WaitForFixedUpdate();"
		yield return BaseCoroutine.WaitFor.Fixedupdate;

		Debug.Log("Show this message in FixedUpdate.");

		yield return null;

		yield return RotateZTest(angle, time, moveObj);
	}
	#endregion
}
