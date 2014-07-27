using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NewCoroutine : BaseCoroutine  {
	#region Variables
	private Stack<IEnumerator> stack = new Stack<IEnumerator>();
	#endregion

	#region Property
	public override object Current {
		get {
			if( this.stack.Count == 0 )
				return null;

			return this.stack.Peek();
		}
	}
	#endregion

	#region Private Function
	protected override void Foward() {
		if( this.nowStep != this.nextStep )
			return ;

		this.nextStep = ExecuteStep.Update;

		IEnumerator item = null;
		
		while( this.stack.Count > 0 ) {
			item = this.stack.Peek();
			
			if( item != null )
				break;
			
			this.stack.Pop();
		}
		
		if( item != null ) {
			if( item.MoveNext() ) {
				object current = item.Current;

				if( current != null ) {
					IEnumerator newItem = current as IEnumerator;
					
					if( newItem != null )
						this.stack.Push(newItem);
					else if( current is WaitForFixedUpdate )
						this.nextStep = ExecuteStep.FixedUpdate;
				}
			} else
				this.stack.Pop();
		}

		if( this.stack.Count == 0 )
			this.state = CoroutineState.Finish;
	}
	#endregion

	#region Behaviour
	public NewCoroutine(IEnumerator item, MonoBehaviour owner = null) {
		this.stack.Push(item);
		this.Owner = owner;
	}
	#endregion
}
