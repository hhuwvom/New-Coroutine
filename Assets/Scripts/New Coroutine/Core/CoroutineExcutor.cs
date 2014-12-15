using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoroutineExcutor : MonoBehaviour {
	#region Variables
	private static CoroutineExcutor singleton = null;

	private List<BaseCoroutine> list = new List<BaseCoroutine>();
	#endregion

	#region Property
	public static CoroutineExcutor Singleton {
		get {
			if( singleton == null ) {
				GameObject newGameObject = new GameObject("Coroutine Exector");

				newGameObject.hideFlags = HideFlags.HideInHierarchy;
				GameObject.DontDestroyOnLoad(newGameObject);

				singleton = newGameObject.AddComponent<CoroutineExcutor>();

				singleton.StartCoroutine("RunLoop");
			}

			return singleton;
		}
	}

	public BaseCoroutine[] AllCoroutine {
		get {
			return list.ToArray();
		}
	}
	#endregion

	#region Public Function
	public static BaseCoroutine Do(IEnumerator item) {
		if( item == null )
			return null;

		BaseCoroutine newItem = item as BaseCoroutine;
		
		if( newItem != null ) {
			CoroutineExcutor.Singleton.list.Add(newItem);
		} else {
			newItem = new NewCoroutine(item);
			CoroutineExcutor.Singleton.list.Add(newItem);
		}

		return newItem;
	}
	#endregion

	#region Private Function
	private void EndOfFrame() {
		int count = list.Count;
		
		for( int i = 0; i < count; ++i ) {
			var item = list[i];
			
			if( item == null )
				continue;
			
			item.EndOfFrame();
		}
	}

	private void ClearItems() {
		int count = list.Count;

		for( int i = count - 1; i >= 0; --i ) {
			var item = list[i];
			
			if( item == null || item.Done ) {
				list.RemoveAt(i);
			}
		}
	}
	#endregion

	#region Coroutine
	IEnumerator RunLoop() {
		while( true ) {
			yield return new WaitForEndOfFrame();

			if( enabled ) {
				EndOfFrame();
				ClearItems();
			}
		}
	}
	#endregion

	#region Behaviour
	private void Update() {
		int count = list.Count;

		for( int i = 0; i < count; ++i ) {
			var item = list[i];

			if( item == null )
				continue;

			item.Update();
		}
	}
	
	private void FixedUpdate() {
		int count = list.Count;
		
		for( int i = 0; i < count; ++i ) {
			var item = list[i];
			
			if( item == null )
				continue;
			
			item.FixedUpdate();
		}		
	}
	#endregion
	

}
