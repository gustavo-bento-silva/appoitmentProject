using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScissorsManager : MonoBehaviour
{
	public bool playOnStart = true;
	public GameObject[] scissors;
	public float time;
	public Vector3 scale;

	public void Start ()
	{
		if (playOnStart) {
			StartCoroutine (PopScissors ());
		}
	}

	public IEnumerator PopScissors ()
	{
		foreach (GameObject scissor in scissors) {
			Debug.Log ("Name: " + scissor.name);
			iTween.ScaleTo (scissor, iTween.Hash ("scale", scale, "time", time, "oncompletetarget", this.gameObject));
			yield return new WaitForSeconds (time - 0.4f);
		}
	}

}
