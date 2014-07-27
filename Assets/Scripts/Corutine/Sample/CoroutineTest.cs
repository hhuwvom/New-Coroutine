using UnityEngine;
using System.Collections;

public class CoroutineTest : MonoBehaviour {
	#region Coroutine
	public IEnumerator RotateZTest(float angle, float time) {
		float fromeAngle = transform.localEulerAngles.z;
		float nowTime = 0;

		while( nowTime < time ) {
			nowTime = Mathf.Min(nowTime + Time.deltaTime, time);

			Vector3 nowAngle = transform.localEulerAngles;

			nowAngle.z = fromeAngle + (angle * nowTime / time);

			transform.localEulerAngles = nowAngle;

			yield return null;
		}
	}

	public IEnumerator MoveTest(Vector3 move, float time) {
		Vector3 fromPosition = transform.localPosition;
		float nowTime = 0;

		while( nowTime < time ) {
			nowTime = Mathf.Min(nowTime + Time.deltaTime, time);

			Vector3 nowPosition = transform.localPosition;

			nowPosition = fromPosition + (move * nowTime / time);

			transform.localPosition = nowPosition;

			yield return null;
		}
	}

	public IEnumerator MoveAndRotateTest(Vector3 move, float angle, float time) {
		yield return MoveTest(move, time);
		yield return new WaitForSeconds(1.0f);
		yield return RotateZTest(angle, time);
	}
	#endregion
}
