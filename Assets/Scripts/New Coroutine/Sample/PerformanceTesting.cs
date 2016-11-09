using UnityEngine;
using System.Collections;

public class PerformanceTesting : MonoBehaviour {
	private void Start() {
		int count = 10000;

//		for( int i = 0; i < count; ++i )
//			StartCoroutine(Flip());

		for( int i = 0; i < count; ++i )
			CoroutineExecutor.Do(Flip());
	}

	IEnumerator Flip() {
		bool flag = false;
		int number = 0;

		while( number >= 0 ) {
			flag = !flag;
			++number;

			yield return null;
		}
	}
}