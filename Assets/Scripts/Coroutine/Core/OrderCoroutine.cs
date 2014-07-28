using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrderCoroutine : BaseCoroutine {
	#region Variables
	private Queue<BaseCoroutine> queue = new Queue<BaseCoroutine>();
	#endregion

	#region Property
	public override object Current {
		get {
			if( queue.Count == 0 )
				return null;

			return queue.Peek();
		}
	}
	#endregion

	#region Public Function
	public void Add(IEnumerator item) {
		BaseCoroutine newItem = item as BaseCoroutine;

		if( newItem != null )
			queue.Enqueue(newItem);
		else
			queue.Enqueue(new NewCoroutine(item));
	}

	public void Add(IEnumerator[] items) {
		if( items == null || items.Length == 0 )
			return ;
		
		for( int i = 0; i < items.Length; ++i ) {
			var item = items[i];
			
			if( item == null )
				return ;
			
			Add(item);
		}
	}
	#endregion

	#region Private Function
	protected override void Foward() {
		BaseCoroutine item = null;

		while( queue.Count > 0 ) {
			item = queue.Peek();

			if( item != null )
				break;

			queue.Dequeue();
		}

		if( item != null ) {
			bool flag = false;

			switch( nowStep ) {
			case ExecuteStep.Update:
				flag = item.Update();
				break;
			case ExecuteStep.FixedUpdate:
				flag = item.FixedUpdate();
				break;
			case ExecuteStep.EndOfFrame:
				flag = item.EndOfFrame();
				break;
			}

			if( !flag )
				queue.Dequeue();
		}

		if( queue.Count == 0 )
			state = CoroutineState.Finish;
	}
	#endregion

	#region Behaviour
	public OrderCoroutine(IEnumerator item, MonoBehaviour owner = null) {
		Add(item);
		Owner = owner;
	}
	
	public OrderCoroutine(IEnumerator[] items, MonoBehaviour owner = null) {
		Add(items);
		Owner = owner;
	}
	#endregion
}
