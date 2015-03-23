using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NewCoroutine : BaseCoroutine  {
	#region Variables
	private Stack<IEnumerator> stack = new Stack<IEnumerator>();

	private float delayTime = 0;
	#endregion

	#region Property
	public override object Current {
		get {
			if( stack.Count == 0 )
				return null;

			return stack.Peek();
		}
	}
	#endregion

	#region Private Function
	protected override void Forward() {
		if( nowStep != nextStep )
			return ;

		nextStep = ExecuteStep.Update;

		// Delay coroutine
		if( nowStep == ExecuteStep.Update ) {
			if( delayTime > 0 ) {
				delayTime = Mathf.Max(0, delayTime - Time.deltaTime);

				if( delayTime > 0 )
					return ;
			}
		}

		IEnumerator item = null;
		
		while( stack.Count > 0 ) {
			item = stack.Peek();
			
			if( item != null )
				break;
			
			stack.Pop();
		}
		
		if( item != null ) {
			if( item.MoveNext() ) {
				object current = item.Current;

				if( current != null ) {
					IEnumerator newItem = current as IEnumerator;
					
					if( newItem != null )
						stack.Push(newItem);
					else if( current is float ) {
						delayTime = Mathf.Max(0, (float)current);
					} else if( current is WaitFor ) {
						WaitFor wait = (WaitFor)current;

						if( wait == WaitFor.FixedUpdate )
							nextStep = ExecuteStep.FixedUpdate;
						else
							nextStep = ExecuteStep.EndOfFrame;
					} else if( current is WaitForFixedUpdate )
						nextStep = ExecuteStep.FixedUpdate;
					else if( current is WaitForEndOfFrame )
						nextStep = ExecuteStep.EndOfFrame;
				}
			} else
				stack.Pop();
		}

		if( stack.Count == 0 )
			state = CoroutineState.Finish;
	}
	#endregion

	#region Behaviour
	public NewCoroutine(IEnumerator item, MonoBehaviour owner = null) {
		stack.Push(item);
		Owner = owner;
	}
	#endregion
}
