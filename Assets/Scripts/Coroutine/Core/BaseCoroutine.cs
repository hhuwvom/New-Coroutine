using UnityEngine;
using System.Collections;

public abstract class BaseCoroutine : IEnumerator {
	#region Enum
	public enum CoroutineState {
		Play,
		Pause,
		Cancel,
		Finish,
	}

	public enum ExecuteStep {
		Update,
		FixedUpdate,
		EndOfFrame,
	}

	public enum WaitFor {
		Fixedupdate = -1,
		EndOfFrame = -2,
	}
	#endregion

	#region Variables
	protected CoroutineState state = CoroutineState.Play;
	private bool hasOwner = false;
	private MonoBehaviour owner = null;
	protected ExecuteStep nowStep = ExecuteStep.Update;
	protected ExecuteStep nextStep = ExecuteStep.Update;
	#endregion

	#region Property
	public MonoBehaviour Owner {
		set {
			owner = value;
			hasOwner = (owner != null);
		}

		get {
			return owner;
		}
	}
	
	public bool Pause {
		set {
			if( state == CoroutineState.Finish ) {
				Debug.Log("Courine is already finished.");
				return ;
			}
			
			if( state == CoroutineState.Cancel ) {
				Debug.Log("Corutine is canceled.");
				return ;
			}
			
			state = (value)? CoroutineState.Pause : CoroutineState.Play;
		}
		
		get {
			return state == CoroutineState.Pause;
		}
	}
	
	public bool Done {
		get {
			return state == CoroutineState.Cancel || state == CoroutineState.Finish;
		}
	}

	public abstract object Current { get; }

	object IEnumerator.Current {
		get {
			return Current;
		}
	}
	#endregion

	#region Behaviour
	protected abstract void Foward();

	public bool MoveNext() {
		if( hasOwner ) {
			if( owner == null )
				state = CoroutineState.Cancel;
			else {
				if( !owner.enabled || !owner.gameObject.activeInHierarchy )
					return true;
			}
		}
		
		switch( state ) {
		case CoroutineState.Cancel:
		case CoroutineState.Finish:
			return false;
		case CoroutineState.Pause:
			return true;
		default:
			break;
		}

		Foward();

		return state != CoroutineState.Finish;
	}

	public void Reset() {
		Debug.LogError(GetType().Name + " does not support Reset().");
	}

	public void Cancel() {
		if( Done )
			return ;

		state = CoroutineState.Cancel;
	}

	public bool Update() {
		nowStep = ExecuteStep.Update;

		return MoveNext();
	}

	public bool FixedUpdate() {
		nowStep = ExecuteStep.FixedUpdate;

		return MoveNext();
	}

	public bool EndOfFrame() {
		nowStep = ExecuteStep.EndOfFrame;

		return MoveNext();
	}
	#endregion
}
