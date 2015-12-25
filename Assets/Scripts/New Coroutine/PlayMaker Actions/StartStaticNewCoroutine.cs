using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions {
	[ActionCategory(ActionCategory.ScriptControl)]
	[Tooltip("Start a static new coroutine in a class.")]
	public class StartStaticNewCoroutine : FsmStateAction {
		#region Variables

		[Tooltip("Full path to the class that contains the static new coroutine.")]
		public FsmString className;

		[Tooltip("The static method to call.")]
		public FsmString methodName;

		[Tooltip("Method parameters. NOTE: these must match the method's signature!")]
		public FsmVar[] parameters;

		[Tooltip("Send event when coroutine finished. If none, action will finish immediately.")]
		public FsmEvent finishEvent;

		[Tooltip("Stop coroutine when exiting.")]
		public FsmBool stopOnExit;

		private Type cachedType;
		private string cachedClassName;
		private string cachedMethodName;
		private MethodInfo cachedMethodInfo;
		private ParameterInfo[] cachedParameterInfo;
		private object[] parametersArray;
		private string errorString;

		private BaseCoroutine coroutine;
		#endregion

		#region Private Function

		private bool DoCache() {
			cachedType = ReflectionUtils.GetGlobalType(className.Value);

			if( cachedType == null ) {
				errorString += "Class is invalid: " + className.Value + "\n";
				Finish();
				return false;
			}

			cachedClassName = className.Value;

#if NETFX_CORE

			cachedMethodInfo = cachedType.GetTypeInfo().GetDeclareMethod(methodName.Value);

#else

			var types = new List<Type>( capacity: parameters.Length );

			foreach( var each in parameters )
				types.Add(each.RealType);
		

			cachedMethodInfo = cachedType.GetMethod(methodName.Value, types.ToArray());

#endif

			if( cachedMethodInfo == null ) {
				errorString += "Invalid Method Name or Parameters: " + methodName.Value + "\n";
				Finish();
				return false;
			}

			cachedMethodName = methodName.Value;
			cachedParameterInfo = cachedMethodInfo.GetParameters();


			return true;
		}

		private void DoCallNewCoroutine() {
			if( className == null || string.IsNullOrEmpty(className.Value) ) {
				Finish();
				return;
			}

			if( cachedClassName != className.Value || cachedMethodName != methodName.Value ) {
				errorString = string.Empty;

				if( !DoCache() ) {
					Debug.LogError(errorString);
					Finish();
					return;
				}
			}

			if( cachedParameterInfo.Length == 0 ) {
				coroutine = CoroutineExecutor.Do((IEnumerator)cachedMethodInfo.Invoke(null, null));
			} else {
				for( int i = 0; i < parameters.Length; ++i ) {
					var parameter = parameters[i];

					parameter.UpdateValue();
					parametersArray[i] = parameter.GetValue();
				}

				coroutine = CoroutineExecutor.Do((IEnumerator)cachedMethodInfo.Invoke(null, parametersArray));
			}

			if( coroutine != null )
				coroutine.Owner = Fsm.Owner;
		}

		#endregion

		#region Behaviour

		public override void OnEnter() {
			parametersArray = new object[parameters.Length];
			coroutine = null;

			DoCallNewCoroutine();

			if( finishEvent == null )
				Finish();
		}

		public override void OnUpdate() {
			if( coroutine == null ) {
				Finish();
			} else if( coroutine.Done ) {
				Fsm.Event(finishEvent);
				Finish();
			}
		}

		public override void OnExit() {
			if( stopOnExit.Value && coroutine != null && !coroutine.Done )
				coroutine.Cancel();
		}

		public override string ErrorCheck() {
			errorString = string.Empty;
			DoCache();

			if( !string.IsNullOrEmpty(errorString) )
				return errorString;

			if( parameters.Length != cachedParameterInfo.Length )
				return "Parameter count does not match method.\nMethod has " + cachedParameterInfo.Length + " parameters.\nYou specified " + parameters.Length + " parameters.";

			for( var i = 0; i < parameters.Length; ++i ) {
				var p = parameters[i];
				var paramType = p.RealType;
				var paramInfoType = cachedParameterInfo[i].ParameterType;

				if( !ReferenceEquals(paramType, paramInfoType) )
					return "Parameters do not match method signature.\nParameter " + (i + 1) + " (" + paramType + ") should be of type: " + paramInfoType;
			}
		

			return string.Empty;
		}

		#endregion
	}
}