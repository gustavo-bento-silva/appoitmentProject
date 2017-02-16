using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System;

namespace PageNavFrameWork{
	
	[CustomPropertyDrawer(typeof(MethodDelegateAttribute))]
	public class MethodDelegateDrawer : PropertyDrawer {
		float rows = 5;
		const float minRows = 5;
		float extraHeight = 0;

		bool descriptionFoldout = false;

		public override void OnGUI (Rect pos, SerializedProperty properties, GUIContent label) {
			//Debug.Log(((attribute as ActionAttribute).paramTypes.Length).ToString());

			SerializedProperty targetProperty = properties.FindPropertyRelative("target");
			SerializedProperty editorTargetProperty = properties.FindPropertyRelative("editorTarget");
			SerializedProperty editorTargetScriptProperty = properties.FindPropertyRelative("editorTargetScript");
			SerializedProperty methodNameProperty = properties.FindPropertyRelative("method"); 
			SerializedProperty candidateNamesProperty = properties.FindPropertyRelative("candidates");
			SerializedProperty indexProperty = properties.FindPropertyRelative("index");
			SerializedProperty monoBehaviourIndexProperty = properties.FindPropertyRelative("monoBehaviourIndex");
			int scriptIndex = monoBehaviourIndexProperty.intValue;

			extraHeight = 0;

			MethodDelegate md = properties.serializedObject.targetObject as System.Object as MethodDelegate;

			if (Event.current.type == EventType.Repaint)
			{
				GUI.skin.box.Draw (pos, new GUIContent(""), 0);
			}

			// pass through label
			EditorGUI.LabelField(
				new Rect (pos.x, pos.y, pos.width, pos.height/rows),
				label
			);

			string description = "Type: ";

			if(getAttribute().paramTypes.Length > 0){
				description += "(";
				foreach(Type type in getAttribute().paramTypes){
					description += type.Name+",";
				}
				description = description.Substring (0, description.Length - 1);
				description += ")";
			}

			description += "->"+getAttribute ().returnType.Name;

			EditorGUI.LabelField(
				
				new Rect (pos.x, pos.y += pos.height/rows, pos.width, pos.height/rows),
				new GUIContent(description)
			);
				
			string descriptionText = getAttribute ().description;
			EditorGUI.indentLevel++;
			descriptionFoldout = EditorGUI.Foldout (new Rect (pos.x, pos.y += pos.height / rows, pos.width, pos.height / rows), descriptionFoldout, "Description");
			if (descriptionFoldout) {
				rows = minRows;
				EditorGUI.indentLevel++;
				float extraRows = 0;
				GUIStyle style = new GUIStyle (GUI.skin.label);
				style.wordWrap = true;
				style.clipping = TextClipping.Clip;
				Vector2 size = GUI.skin.label.CalcSize (new GUIContent (descriptionText));
				extraRows = size.x / pos.width;
				EditorGUI.LabelField (new Rect (pos.x, pos.y += size.y, pos.width, size.y * (extraRows + 1)), descriptionText, style);
				pos.y += size.y * (extraRows);
				rows += extraRows+1;
				EditorGUI.indentLevel--;
			} else {
				rows = minRows;
				extraHeight = 0;
			}
			if (editorTargetProperty.objectReferenceValue is GameObject) {
				rows += 1;
			}

			EditorGUI.indentLevel--;

			// target + method section
			EditorGUI.BeginChangeCheck(); // if target changes we need to repopulate the candidate method lists

			// select target
			EditorGUI.PropertyField(
				new Rect (pos.x, pos.y += pos.height/rows, pos.width, pos.height/rows),
				editorTargetProperty,
				new GUIContent("Target")
			);

			if (editorTargetProperty.objectReferenceValue is GameObject) {
				List<string> MonoBehaviourNames = new List<string> ();
				MonoBehaviour[] MonoBehaviourCandidateList = ((GameObject)editorTargetProperty.objectReferenceValue).gameObject.GetComponents<MonoBehaviour> ();
				if (editorTargetProperty.objectReferenceValue is GameObject) {
					foreach (MonoBehaviour mb in MonoBehaviourCandidateList) {
						MonoBehaviourNames.Add (mb.GetType ().Name);
					}
				}

				if(scriptIndex >= MonoBehaviourNames.Count){
					scriptIndex = 0;
				}

				// select method from candidates
				scriptIndex = EditorGUI.Popup (
					new Rect (pos.x, pos.y += pos.height / rows, pos.width, pos.height / rows),
					"Script:",
					scriptIndex,
					MonoBehaviourNames.ToArray ()
				);

				monoBehaviourIndexProperty.intValue = scriptIndex;

				if (scriptIndex < MonoBehaviourCandidateList.Length) {
					targetProperty.objectReferenceValue = MonoBehaviourCandidateList [scriptIndex];
				} else {
					if (MonoBehaviourCandidateList.Length > 0) {
						targetProperty.objectReferenceValue = MonoBehaviourCandidateList [0];
					} else {
						targetProperty.objectReferenceValue = null;
					}
				}
			} else {
				targetProperty.objectReferenceValue = editorTargetProperty.objectReferenceValue;
			}
			

			if(targetProperty.objectReferenceValue == null) {
				return; // null objects have no methods - don't continue
			} 

			// polulate method candidate names
			string[] methodCandidateNames;

			// lets do some reflection work -> search, filter, collect candidate methods..
			methodCandidateNames = RepopulateCandidateList (properties);

			//copy values to array in the object
			candidateNamesProperty.ClearArray ();
			candidateNamesProperty.arraySize = methodCandidateNames.Length;
			// assign storage containers
			int i = 0;
			foreach(SerializedProperty element in candidateNamesProperty) {
				element.stringValue = methodCandidateNames[i];
			}


			// place holder when no candidates are available
			if (methodCandidateNames.Length == 0) {
				EditorGUI.LabelField (
					new Rect (pos.x, pos.y += pos.height / rows, pos.width, pos.height / rows),
					"Method",
					"none"
				);    
				return; // no names no game
			}

			// select method from candidates
			indexProperty.intValue = EditorGUI.Popup (
				new Rect (pos.x, pos.y += pos.height/rows, pos.width, pos.height/rows),
				"Method (" + targetProperty.objectReferenceValue.GetType().ToString() + ")",
				indexProperty.intValue,
				methodCandidateNames
			);

			methodNameProperty.stringValue = methodCandidateNames[indexProperty.intValue];
			EditorGUI.indentLevel--;
		}    

		public string[] RepopulateCandidateList(SerializedProperty sp) {
			UnityEngine.Object target = sp.FindPropertyRelative("target").objectReferenceValue;
			if(target == null){
				return new string[0];
			}
			System.Type type = target.GetType();
			System.Type[] paramTypes = getAttribute().paramTypes;
			IList<MethodInfo> candidateList = new List<MethodInfo>();
			string[] candidateNames;

			List<string> methodNames = new List<string> ();

			foreach (MethodInfo info in type.GetMethods()) {
				if(CheckMethodForFitting(getAttribute().returnType,paramTypes,info)){
					methodNames.Add (info.Name);
				}
			}

			return methodNames.ToArray();
		}

		public bool CheckMethodForFitting(Type returnType, Type[] paramType, MethodInfo methodInfo){
			if(methodInfo.GetParameters().Length != paramType.Length){
				return false;
			}
			if(returnType != methodInfo.ReturnParameter.ParameterType){
				return false;
			}
			int i = 0;
			if(methodInfo.GetParameters().Length == 0){
				return true;
			}
			foreach(ParameterInfo info in methodInfo.GetParameters()){
				if(paramType[i] != info.ParameterType){
					return false;
				}
				i++;
			}
			return true;
		}

		public MethodDelegateAttribute getAttribute(){
			return (MethodDelegateAttribute)attribute;
		}
			
		public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
			return base.GetPropertyHeight (property, label) * rows+ extraHeight + 5;
		}
	}
}