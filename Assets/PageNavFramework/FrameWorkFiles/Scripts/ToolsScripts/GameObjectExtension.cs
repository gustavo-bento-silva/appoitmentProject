using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using UnityEngine.UI;

public static class GameObjectExtension {

	public enum Axis {X, Y};

	public delegate void Callback();

	public static void MoveTo (this GameObject go, Vector3 position, float time, string effect = "linear")
	{
		iTween.MoveTo (go, iTween.Hash ("position", position, "time", time, "easetype", effect, "islocal", true));
	}		

	public static void MoveTo (this GameObject go, Axis axis, float position, float time, string effect = "linear")
	{
		if(axis == Axis.X)
		{
			iTween.MoveTo (go, iTween.Hash ("x", position, "time", time, "easetype", effect, "islocal", true));
		}
		else
		{
			iTween.MoveTo (go, iTween.Hash ("y", position, "time", time, "easetype", effect, "islocal", true));
		}
	}

	public static void ScaleTo (this GameObject go, Vector3 scale, float time, string effect = "linear")
	{
		iTween.ScaleTo (go, iTween.Hash ("scale", scale, "time", time, "easetype", effect));
	}

	public static void FadeTo (this GameObject go, float alpha, float time)
	{
		go.GetComponent<Image>().CrossFadeAlpha(alpha, time, false);
	}

	public static bool AlphaIsEnabled(this GameObject go)
	{
		if(go.GetComponent<Image>().color.a == 1) return true;
		else return false;
	}
		
}

public static class MonoBehaviourExtension {
	public static void InvokeWithArgument(this MonoBehaviour mb,string name, Object o)
	{
		Object[] ob = { o };
		if(mb){
			mb.GetType ().GetMethod (name).Invoke (mb, ob);
		}
	}
}
