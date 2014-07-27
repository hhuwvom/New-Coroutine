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
			this.list.Add(newItem);
		else
			this.list.Add(new NewCoroutine(item));
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
		int count = this.list.Count;
		bool flag = false;

		for( int i = 0; i < count; ++i ) {
			var item = this.list[i];

			if( item == null )
				continue;

			switch( this.nowStep ) {
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
			this.state = CoroutineState.Finish;
	}
	#endregion

	#region Behaviour
	public GroupCoroutine(IEnumerator item, MonoBehaviour owner = null) {
		Add(item);
		this.Owner = owner;
	}

	public GroupCoroutine(IEnumerator[] items, MonoBehaviour owner = null) {
		Add(items);
		this.Owner = owner;
	}
	#endregion
}
