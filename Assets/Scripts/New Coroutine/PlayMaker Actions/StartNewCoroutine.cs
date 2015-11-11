using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using Object = UnityEngine.Object;
using HutongGames.PlayMaker;

namespace HutongGames.PlayMaker.Actions {
	[ActionCategory(ActionCategory.ScriptControl)]
	public class StartNewCoroutine : FsmStateAction {
		#region Variables
		[ObjectType(typeof(MonoBehaviour))]
		[Tooltip("Store the component in an Object variable.\nNOTE: Set theObject variable's Object Type to get a component of that type. E.g., set Object Type to UnityEngine.AudioListener to get the AudioListener component on the camera.")]
		public FsmObject behaviour;

		[Tooltip("Name of the method to call on the component. Must be coroutine.")]
		public FsmString methodName;
		[Tooltip("Method paramters. NOTE: these must match the method's signature!")]
		public FsmVar[] parameters;

		[Tooltip("Send event when coroutine finished. If none, action will finish immediately.")]
		public FsmEvent finishEvent;

		[Tooltip("Stop coroutine when exiting.")]
		public FsmBool stopOnExit;

		private Object cachedBehaviour;
		private Type cachedType;
		private MethodInfo cachedMethodInfo;
		private ParameterInfo[] cachedParameterInfo;
		private object[] parameterArray;
		private string errorString;

		private BaseCoroutine coroutine;
		#endregion

		#region Private Function
		private bool DoCache() {
			cachedBehaviour = behaviour.Value as MonoBehaviour;

			if( cachedBehaviour == null ) {
				errorString = "Behaviour is invalid!\n";
				Finish();
				return false;
			}

			cachedType = behaviour.Value.GetType();

#if NETFX_CORE
			cachedMethodInfo = cachedType.GetTypeInfo().GetDeclareMethod(methodName.Value);
#else
			cachedMethodInfo = cachedType.GetMethod(methodName.Value);
#endif

			if( cachedMethodInfo == null ) {
				errorString += "Method Name is invalid: " + methodName.Value + "\n";
				Finish();
				return false;
			}

			cachedParameterInfo = cachedMethodInfo.GetParameters();

			return true;
		}

		private void DoCallNewCoroutine() {
			if( behaviour.Value == null ) {
				Finish();
				return ;
			}

			if( cachedBehaviour != behaviour.Value ) {
				errorString = string.Empty;

				if( !DoCache() ) {
					Debug.LogError(errorString);
					Finish();
					return ;
				}
			}

			if( cachedParameterInfo.Length == 0 ) {
				coroutine = CoroutineExecutor.Do((IEnumerator)cachedMethodInfo.Invoke(cachedBehaviour, null));
			} else {
				for( int i = 0; i < parameters.Length; ++i ) {
					var parameter = parameters[i];

					parameter.UpdateValue();
					parameterArray[i] = parameter.GetValue();
				}

				coroutine = CoroutineExecutor.Do((IEnumerator)cachedMethodInfo.Invoke(cachedBehaviour, parameterArray));
			}

			if( coroutine != null )
				coroutine.Owner = (MonoBehaviour)cachedBehaviour;
		}
		#endregion

		#region Fsm
		public override void Reset() {
			behaviour = null;
		 }

		public override void OnEnter() {
			parameterArray = new object[parameters.Length];
			coroutine = null;

			DoCallNewCoroutine();

			if( finishEvent == null )
				Finish();
		}

		public override void OnUpdate () {
			if( coroutine != null && coroutine.Done ) {
				Fsm.Event(finishEvent);
				Finish();
			}
		}

		public override void OnExit () {
			if( stopOnExit.Value && coroutine != null && !coroutine.Done )
				coroutine.Cancel();
		}

		public override string ErrorCheck () {
			errorString = string.Empty;

			DoCache();

			if( !string.IsNullOrEmpty(errorString) )
				return errorString;

			if( parameters.Length != cachedParameterInfo.Length )
				return "Parameter count does ont match method.\nMethod has " + cachedParameterInfo.Length + " parameters.\nYou specified " + parameters.Length + " parameters.";

			for( int i = 0; i < parameters.Length; ++i ) {
				var p = parameters[i];
				var paramType = p.RealType;
				var paramInfoType = cachedParameterInfo[i].ParameterType;

				if( !ReferenceEquals(paramType, paramInfoType) )
					return "Parameters do not match method signature.\nParameter " + (i + 1) + " ( " + paramType + ") should be of type: " + paramInfoType;
			}

			if( !ReferenceEquals(cachedMethodInfo.ReturnType, typeof(IEnumerator)) )
			   return "This method should be coroutine.";

			return string.Empty;
		}
		#endregion
	}
}