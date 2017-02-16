using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.UI;

namespace PageNavFrameWork{
	public class ToolsHotKeyScript : MonoBehaviour{

		[MenuItem("PageNav/Tools/Duplicate Selected GameObjects In Order %#d")]
		public static void DuplicateGameObjectsInOrderFunction(){
			var selectList = Selection.gameObjects;
			List<GameObject> list = new List<GameObject>(selectList);
			list.Sort ((GameObject x, GameObject y) => (x.transform.GetSiblingIndex()-y.transform.GetSiblingIndex()));
			GameObject l;
			selectList = list.ToArray ();
			for(int i = 0; i<selectList.Length;i++){
				if(selectList[i] is GameObject){
					var newGo = Instantiate (selectList[i]);
					newGo.transform.SetParent (selectList[i].transform.parent,false);
					newGo.name = selectList [i].name;
					GetNewName (newGo);
					Undo.RegisterCreatedObjectUndo (newGo,"Created New Copy");
				}
			}
		}

		public static int CompareNames(string x, string y){
			if (x.Length < y.Length) {
				return -1;
			} else if (x.Length > y.Length) {
				return 1;
			} else {
				return x.CompareTo (y);
			}
		}

		public static void GetNewName(GameObject go){
			int acc = 0;
			string newName = "";
			while(true){
				if(Regex.IsMatch(go.name,".* [(][0-9]+[)]")){
					newName = go.name;
					newName = go.name.Substring(0,go.name.LastIndexOf("(")+1)+acc+")";
				}else{
					newName = go.name + " ("+acc+")";
				}
				if(!SearchForChildNamed(go.transform.parent,newName)){
					break;
				}
				acc++;
			}
			go.name = newName;
		}

		public static bool SearchForChildNamed(Transform parent,string search){
			if(parent == null){
				if (GameObject.Find (search) == null) {
					return false;
				} else {
					return true;
				}
			}
			foreach(Transform t in parent){
				if(t.gameObject.name == search){
					return true;
				}
			}
			return false;
		}

		[MenuItem("PageNav/Tools/UI/RectTransform/Snap Anchors To Corners %#a")]
		public static void SnapAnchorsToCorners(){
			GameObject selectedGO = Selection.activeGameObject;
			if(!(selectedGO.transform is RectTransform)){//if not recttransform
				Debug.Log("Selected GameObject does not have a RectTransform");
				return;
			}
			Undo.RegisterCompleteObjectUndo (selectedGO.transform,"Snap Anchors to corners");
			AnchorToCornersScript.gameObject = selectedGO;
			AnchorToCornersScript.executeScript ();
		}

		[MenuItem("PageNav/Tools/Toggle Active State GameObject %t")]
		public static void Temp(){
			var selectList =  Selection.gameObjects;
			Undo.RecordObjects (selectList,"Toggled active GameObject");
			foreach(GameObject go in selectList){
				go.SetActive (!go.activeSelf);
			}
		}

		[MenuItem("PageNav/Tools/UI/RectTransform/Copy RectTrasnform %&c")]
		public static void CopyRectTransform() {
			var selection = Selection.activeGameObject;
			if(selection == null){
				TransferRectTransform = null;
				return;
			}
			if(!(selection.transform is RectTransform)){//if not recttransform
				Debug.Log("Selected GameObject does not have a RectTransform");
				TransferRectTransform = null;
				return;
			}
			if(TransferRectTransform!=null){
				DestroyImmediate (TransferRectTransform.gameObject);
			}
			TransferRectTransform = Instantiate (selection.transform) as RectTransform;
			TransferRectTransform.hideFlags = HideFlags.HideAndDontSave;
			TransferRectTransform.SetParent (selection.transform.parent,false);
			TransferRectTransform.gameObject.name = TransferGameObjectName;
		}

		const string TransferGameObjectName = "TransferGameObject";
		public static RectTransform TransferRectTransform = null;
		[MenuItem("PageNav/Tools/UI/RectTransform/Paste RectTrasnform %&v")]
		public static void PasteractTransform() {
			if(TransferRectTransform == null){
				return;
			}

			var selectedGO = Selection.activeGameObject;
			if(!(selectedGO.transform is RectTransform)){//if not recttransform
				Debug.Log("Selected GameObject does not have a RectTransform");
				return;
			}
			Undo.RegisterCompleteObjectUndo (selectedGO,"Copy RectTransform");
			Transform originParent = selectedGO.transform.parent;
			int siblinIndexOriginal = selectedGO.transform.GetSiblingIndex();
			selectedGO.transform.SetParent (TransferRectTransform.parent);
			List<RectTransform> list = new List<RectTransform> ();
			for(int i=0;i<selectedGO.transform.childCount;i++){
				Transform t = selectedGO.transform.GetChild(i);
				list.Add (t as RectTransform);
			}

			foreach(RectTransform rt in list){
				rt.SetParent (selectedGO.transform.parent);
			}

			RectTransform rect = selectedGO.transform as RectTransform;
			rect.position = TransferRectTransform.position;
			rect.localScale = TransferRectTransform.localScale;
			rect.rotation = TransferRectTransform.rotation;
			rect.anchorMax = TransferRectTransform.anchorMax;
			rect.anchorMin = TransferRectTransform.anchorMin;
			rect.offsetMax = TransferRectTransform.offsetMax;
			rect.offsetMin = TransferRectTransform.offsetMin;
			rect.SetParent (originParent);
			rect.SetSiblingIndex (siblinIndexOriginal);
			foreach(RectTransform rt in list){
				rt.SetParent (selectedGO.transform);
			}
		}

		[MenuItem("PageNav/Tools/Disconnect From Prefab %#p")]
		public static void DisconnectFromPrefab(){
			GameObject selectedGO = Selection.activeGameObject;
			if(!(selectedGO.transform is RectTransform)){//if not recttransform
				Debug.Log("Selected GameObject does not have a RectTransform");
				return;
			}
			Undo.RegisterCompleteObjectUndo (selectedGO.transform,"Snap Anchors to corners");
			PrefabUtility.DisconnectPrefabInstance (selectedGO);
		}

	}
}