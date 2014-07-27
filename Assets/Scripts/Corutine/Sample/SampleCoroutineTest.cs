using UnityEngine;
using System.Collections;

public class SampleCoroutineTest : MonoBehaviour {
	#region Variables
	public CoroutineTest test;
	public CoroutineTest owner;
	public BaseCoroutine doCoroutine;

	private Vector3 originalPosition = Vector3.zero;
	#endregion

	#region Behaviour
	private void Start() {
		originalPosition = transform.position;
	}

	private void Update() {
		if( this.doCoroutine == null )
			return ;

		if( this.doCoroutine.Done )
			this.doCoroutine = null;
	}

	private void OnGUI() {
		if( test == null )
			return ;

		float ox = 10, oy = 10, dx = 160, dy = 60;
		Rect uiPos = new Rect(ox, oy, 150, 40);

		if( this.doCoroutine == null ) {
			if( GUI.Button(uiPos, "RotationZ") ) {
				this.doCoroutine = CoroutineExcutor.Do(test.RotateZTest(270.0f, 2.0f));
			}

			uiPos.x += dx;

			if( GUI.Button(uiPos, "MoveAndRotate") ) {
				this.doCoroutine = CoroutineExcutor.Do(test.MoveAndRotateTest(new Vector3(1.5f, 0.8f, 0), 90.0f, 1.5f));
			}

			uiPos.x = ox;
			uiPos.y += dy;

			if( GUI.Button(uiPos, "Group Coroutine") ) {
				GroupCoroutine group = new GroupCoroutine(test.RotateZTest(270.0f, 2.0f));

				group.Add(test.MoveTest(new Vector3(1.5f, 0.8f, 0), 3.0f));

				this.doCoroutine = CoroutineExcutor.Do(group);
			}

			uiPos.x += dx;

			if( GUI.Button(uiPos, "Order Coroutine") ) {
				OrderCoroutine order = new OrderCoroutine(test.RotateZTest(270.0f, 2.0f));

				order.Add(test.MoveTest(new Vector3(1.5f, 0.8f, 0), 3.0f));

				this.doCoroutine = CoroutineExcutor.Do(order);
			}

			uiPos.x = ox;
			uiPos.y += dy;

			if( GUI.Button(uiPos, "Complex Coroutine") ) {
				OrderCoroutine order = new OrderCoroutine(test.MoveTest(new Vector3(-1.5f, -0.8f, -3.0f), 1.0f));

				GroupCoroutine group = new GroupCoroutine(new IEnumerator[] {
					test.RotateZTest(270.0f, 2.0f),
					test.MoveTest(new Vector3(1.5f, 0.8f, 3.0f), 3.0f)});

				order.Add(group);

				this.doCoroutine = CoroutineExcutor.Do(order);
			}

			uiPos.x += dx;

			if( GUI.Button(uiPos, "Coroutine with Owner") ) {
				if( this.owner == null ) {
					Debug.LogError("You need to set owner first.");
					return ;
				}

				OrderCoroutine order = new OrderCoroutine(this.owner.MoveTest(new Vector3(-1.5f, -0.8f, -3.0f), 1.0f, transform), this.owner);
				
				GroupCoroutine group = new GroupCoroutine(new IEnumerator[] {
					this.owner.RotateZTest(270.0f, 2.0f, transform),
					this.owner.MoveTest(new Vector3(1.5f, 0.8f, 3.0f), 3.0f, transform)
				});
				
				order.Add(group);
				
				this.doCoroutine = CoroutineExcutor.Do(order);
			}

			uiPos.x = ox;
			uiPos.y += dy;

			if( GUI.Button(uiPos, "Reset Position") ) {
				transform.position = originalPosition;
			}

		} else {
			if( this.doCoroutine.Pause ) {
				if( GUI.Button(uiPos, "Resume") ) {
					this.doCoroutine.Pause = false;
				}
			} else {
				if( GUI.Button(uiPos, "Pause") ) {
					this.doCoroutine.Pause = true;
				}
			}

			if( this.doCoroutine.Owner != null ) {
				uiPos.x += dx;

				if( this.doCoroutine.Owner.enabled ) {
					if( GUI.Button(uiPos, "Disable Owner") )
						this.doCoroutine.Owner.enabled = false;
				} else {
					if( GUI.Button(uiPos, "Enable Owner") )
						this.doCoroutine.Owner.enabled = true;
				}

				uiPos.x += dx;

				if( GUI.Button(uiPos, "Kill Owner") )
					Destroy(this.doCoroutine.Owner);
			}

			uiPos.x += dx;

			if( GUI.Button(uiPos, "Cancel") ) {
				this.doCoroutine.Cancel();
			}
		}
	}
	#endregion
}
