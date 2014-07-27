using UnityEngine;
using System.Collections;

public class TimeCoroutine {
	#region Public Static Function
	public static IEnumerator WaitForSeconds(float seconds) {
		while( seconds > 0 ) {
			seconds -= Time.deltaTime;
			
			yield return null;
		}
	}
	#endregion
}
