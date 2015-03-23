using UnityEngine;
using System.Collections;

public class EventCoroutine : BaseCoroutine {
	#region Variables
	private string[] events;

	public string CurrentEvent { get; private set; }
	#endregion

	#region Property
	public override object Current {
		get {
			return null;
		}
	}
	#endregion

	#region Public Function
	public void SendEvent(string ev) {
		CurrentEvent = ev;

		Debug.Log("Get event " + ev);

		if( events == null )
			return ;

		var count = events.Length;

		for( int i = 0; i < count; ++i ) {
			if( events[i] != ev )
				continue;

			FinishEvent();
			break;
		}
	}
	#endregion
	
	#region Private Function
	protected override void Forward() {
		if( events == null ) {
			FinishEvent();
			return ;
		}
	}

	private void FinishEvent() {
		state = CoroutineState.Finish;
		CoroutineExcutor.Singleton.ProcessEvent -= SendEvent;
	}
	#endregion

	#region Behaviour
	public EventCoroutine(string[] evs, MonoBehaviour owner = null) {
		events = evs;
		Owner = owner;

		CoroutineExcutor.Singleton.ProcessEvent += SendEvent;
	}
	#endregion
}
