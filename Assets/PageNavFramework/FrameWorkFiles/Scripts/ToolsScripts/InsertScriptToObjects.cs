#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

public class InsertScriptToObjects {

//	[MenuItem ("Tools/Add Script to Objects")]
//	static void AddingScript()
//	{
//		foreach (LevelButtonControl obj in Selection.activeTransform.GetComponentsInChildren<LevelButtonControl>())
//		{
//			if(obj.GetComponent<Pulse>() == null)
//			{
//				obj.gameObject.AddComponent<Pulse>();
//				Pulse pulse = obj.GetComponent<Pulse>();
//				pulse.percentageIncrease = 1.2f;
//				pulse.animationTime = 0.6f;
//			}
//		}
//	}
//
//	[MenuItem ("Tools/Change Script to Objects")]
//	static void ChangeScript()
//	{		
//		foreach (LevelButtonControl obj in Selection.activeTransform.GetComponentsInChildren<LevelButtonControl>())
//		{
//			Pulse pulse = obj.gameObject.GetComponent<Pulse>();
//			if(pulse)pulse.percentageIncrease = 1.2f;
//		}
//	}
}
#endif
