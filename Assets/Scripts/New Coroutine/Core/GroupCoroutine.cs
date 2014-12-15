using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroupCoroutine : BaseCoroutine {
	#region Variables
	public List<BaseCoroutine> list = new List<BaseCoroutine>();
	#endregion

	#region Property
	public override object Current {
		get {
			return null;
		}
	}
	#endregion

	#region Public Function
	public void Add(IEnumerator item) {
		BaseCoroutine newItem = item as BaseCoroutine;

		if( newItem != null )
			list.Add(newItem);
		else
			list.Add(new NewCoroutine(item));
	}

	public void Add(IEnumerator[] items) {
		if( items == null || items.Length == 0 )
			return ;

		for( int i = 0; i < items.Length; ++i ) {
			var item = items[i];

			if( item == null )
				continue;

			Add(item);
		}
	}
	#endregion

	#region Private Function
	protected override void Foward() {
		int count = list.Count;
		bool flag = false;

		for( int i = 0; i < count; ++i ) {
			var item = list[i];

			if( item == null )
				continue;

			switch( nowStep ) {
			case ExecuteStep.Update:
				flag |= item.Update();
				break;
			case ExecuteStep.FixedUpdate:
				flag |= item.FixedUpdate();
				break;
			case ExecuteStep.EndOfFrame:
				flag |= item.EndOfFrame();
				break;
			}
		}

		if( !flag )
			state = CoroutineState.Finish;
	}
	#endregion

	#region Behaviour
	public GroupCoroutine(IEnumerator item, MonoBehaviour owner = null) {
		Add(item);
		Owner = owner;
	}

	public GroupCoroutine(IEnumerator[] items, MonoBehaviour owner = null) {
		Add(items);
		Owner = owner;
	}
	#endregion
}
