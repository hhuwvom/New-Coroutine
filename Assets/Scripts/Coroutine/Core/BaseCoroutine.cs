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
			this.owner = value;
			this.hasOwner = (this.owner != null);
		}

		get {
			return this.owner;
		}
	}
	
	public bool Pause {
		set {
			if( this.state == CoroutineState.Finish ) {
				Debug.Log("Courine is already finished.");
				return ;
			}
			
			if( this.state == CoroutineState.Cancel ) {
				Debug.Log("Corutine is canceled.");
				return ;
			}
			
			this.state = (value)? CoroutineState.Pause : CoroutineState.Play;
		}
		
		get {
			return this.state == CoroutineState.Pause;
		}
	}
	
	public bool Done {
		get {
			return this.state == CoroutineState.Cancel || this.state == CoroutineState.Finish;
		}
	}

	public abstract object Current { get; }

	object IEnumerator.Current {
		get {
			return this.Current;
		}
	}
	#endregion

	#region Behaviour
	protected abstract void Foward();

	public bool MoveNext() {
		if( this.hasOwner ) {
			if( this.owner == null )
				this.state = CoroutineState.Cancel;
			else {
				if( !this.owner.enabled || !this.owner.gameObject.activeInHierarchy )
					return true;
			}
		}
		
		switch( this.state ) {
		case CoroutineState.Cancel:
		case CoroutineState.Finish:
			return false;
		case CoroutineState.Pause:
			return true;
		default:
			break;
		}

		Foward();

		return this.state != CoroutineState.Finish;
	}

	public void Reset() {
		Debug.LogError(this.GetType().Name + " is not support Reset().");
	}

	public void Cancel() {
		if( Done )
			return ;

		this.state = CoroutineState.Cancel;
	}

	public bool Update() {
		this.nowStep = ExecuteStep.Update;

		return MoveNext();
	}

	public bool FixedUpdate() {
		this.nowStep = ExecuteStep.FixedUpdate;

		return MoveNext();
	}

	public bool EndOfFrame() {
		this.nowStep = ExecuteStep.EndOfFrame;

		return MoveNext();
	}
	#endregion
}
