using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoroutineExecutor : MonoBehaviour {
	#region Variables
	private static CoroutineExecutor singleton = null;

	private List<BaseCoroutine> list = new List<BaseCoroutine>();
	private WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

	public delegate void ProcessEventDelegate(string ev);
	public event ProcessEventDelegate ProcessEvent;
	#endregion

	#region Property
	public static CoroutineExecutor Singleton {
		get {
			if( singleton == null ) {
				GameObject newGameObject = new GameObject("Coroutine Exector");

				newGameObject.hideFlags = HideFlags.HideInHierarchy;
				GameObject.DontDestroyOnLoad(newGameObject);

				singleton = newGameObject.AddComponent<CoroutineExecutor>();
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
			CoroutineExecutor.Singleton.list.Add(newItem);
		} else {
			newItem = new NewCoroutine(item);
			CoroutineExecutor.Singleton.list.Add(newItem);
		}

		newItem.Update();

		return newItem;
	}

	public static void SendEvent(string ev) {
		var proc = CoroutineExecutor.Singleton.ProcessEvent;

		if( proc == null )
			return ;

		proc(ev);
	}
	#endregion

	#region Private Function
	private bool CheckEndOfFrme() {
		int count = list.Count;
		
		for( int i = 0; i < count; ++i ) {
			if( list[i].NextStep == BaseCoroutine.ExecuteStep.EndOfFrame )
				return true;
		}

		return false;
	}

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
	IEnumerator RunEndOfFrame() {
		yield return waitForEndOfFrame;

		EndOfFrame();
	}
	#endregion

	#region Behaviour
	private void Update() {
		ClearItems();

		int count = list.Count;

		for( int i = 0; i < count; ++i ) {
			var item = list[i];

			if( item == null )
				continue;

			item.Update();
		}
	}

	private void LateUpdate() {
		if( CheckEndOfFrme() )
			StartCoroutine(RunEndOfFrame());
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
