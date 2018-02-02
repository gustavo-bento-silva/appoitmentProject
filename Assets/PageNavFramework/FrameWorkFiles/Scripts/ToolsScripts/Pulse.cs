using UnityEngine;
using System.Collections;

public class Pulse : MonoBehaviour {

	public float percentageIncrease;
	public float animationTime;
	public bool isEnabled;
	public iTween.EaseType ease = iTween.EaseType.easeInOutSine;

	private bool isFirstTime = true;
	private Vector3 originalScale;
	private Vector3 increasingScale;

	void Awake () 
	{
		originalScale = gameObject.transform.localScale;
		increasingScale = originalScale * percentageIncrease;
	}
	
	void Update () 
	{
		if(isEnabled && isFirstTime)
		{
			isFirstTime = false;
			InitPulse();
		}
		else if(!isEnabled && !isFirstTime)
		{
			isFirstTime = true;
		}
	}

	void InitPulse()
	{
		if(isEnabled)
		{
			IncreaseSize();
		}
	}

	void IncreaseSize()
	{
		if(isEnabled)
		{
			iTween.ScaleTo( gameObject, iTween.Hash ("scale", increasingScale, "time", animationTime, "easetype", ease.ToString(), "oncompletetarget", this.gameObject, "oncomplete", "DecreaseSize"));
		}
	}

	void DecreaseSize ()
	{
		iTween.ScaleTo( gameObject, iTween.Hash ("scale", originalScale, "time", animationTime, "easetype", ease.ToString(), "oncompletetarget", this.gameObject, "oncomplete", "IncreaseSize"));
	}
}
