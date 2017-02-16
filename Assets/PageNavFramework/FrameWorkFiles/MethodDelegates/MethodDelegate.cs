using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using System.Linq;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;

namespace PageNavFrameWork{
	
	[Serializable]
	public class MethodDelegate{
		// public settings
		[HideInInspector]
		public UnityEngine.Object target;
		[HideInInspector]
		public UnityEngine.Object editorTarget;
		[HideInInspector]
		public UnityEngine.Object editorTargetScript;
		[HideInInspector]
		public string method = "";
		[HideInInspector]
		public string TargetName="";
		[HideInInspector]
		public int monoBehaviourIndex = 0;


		// inspector cache
		[HideInInspector]
		public string[] candidates = {};
		[HideInInspector]
		public int index = 0;

		public System.Object CallDelegate(params object[] parameterList){
			if(!target){
				Debug.LogWarning("MethodDelegate is missing target.");
				return null;
			}
			var methodInfo = target.GetType ().GetMethod (method);
			if(methodInfo==null){
				Debug.LogWarning("MethodDelegate target has no method called \""+method+"\"");
			}
			return methodInfo.Invoke(target,parameterList);
		}

		/// <summary>
		/// Register a method from a target as the delegate. IMPORTANT: The method MUST have the same signature as the MethodDelegate annotation, or else it will cause undefined behaviour.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="methodName">Method name.</param>
		public bool RegisterDelegate(UnityEngine.Object target, string methodName){
			var mInfo = target.GetType ().GetMethod (methodName);
			if(mInfo == null){
				return false;
			}
			this.target = target;
			this.method = methodName;
			return true;
		}

	}

	[AttributeUsage(AttributeTargets.Field)]
	public class MethodDelegateAttribute : PropertyAttribute{

		public Type returnType;
		public Type[] paramTypes;
		public string description = "";

		public MethodDelegateAttribute(string description, Type returnType, params Type[] paramTypes){
			this.returnType = returnType;
			this.paramTypes = paramTypes;
			this.description = description;
		}

		public MethodDelegateAttribute(string description,Type returnType){
			this.returnType = returnType;
			this.paramTypes = new Type[]{};
			this.description = description;
		}

		public MethodDelegateAttribute(string description){
			this.returnType = typeof(void);
			this.paramTypes = new Type[]{};
			this.description = description;
		}

	}
}